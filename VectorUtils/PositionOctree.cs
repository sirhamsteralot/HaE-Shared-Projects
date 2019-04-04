using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public class PositionOctree<T>
        {
            private Sector _root;
            private Vector3D center = Vector3D.Zero;
            private double range = double.MaxValue;
            private int maxDepth;


            public PositionOctree(Vector3D center, double range, int maxDepth)
            {
                this.center = center;
                this.range = range;
                this.maxDepth = maxDepth;
                _root = new Sector(center, range, null, this, 0, -1);
            }

            public PositionOctree()
            {
                maxDepth = 50;
                _root = new Sector(Vector3D.Zero, double.MaxValue, null, this, 0, -1);
            }

            public Leaf? FindClosestLeaf(Vector3D position)
            {
                return _root.FindClosestLeaf(ref position, null);
            }

            public bool TryFindClosestLeaf(Vector3D position, ref Leaf leaf)
            {
                var leafI = _root.FindClosestLeaf(ref position, null);

                if (leafI.HasValue)
                {
                    leaf = leafI.Value;
                    return true;
                }
                return false;
            }

            public bool TryFindExactLeaf(Vector3D position, ref Leaf leaf)
            {
                var leafI = _root.FindExactLeaf(ref position);

                if (leafI.HasValue)
                {
                    leaf = leafI.Value;
                    return true;
                }
                return false;
            }

            public Leaf? FindExactLeaf(Vector3D position)
            {
                return _root.FindExactLeaf(ref position);
            }

            public bool TryAddLeaf(Vector3D position, T payload)
            {
                if (!IsPosValid(position))
                    return false;

                var leaf = new Leaf(position, payload);

                return _root.TryAddLeaf(ref leaf);
            }

            public bool TryDeleteLeafAt(Vector3D position)
            {
                if (!IsPosValid(position))
                    return false;

                var leaf = FindExactLeaf(position);
                if (leaf.HasValue)
                {
                    var mancpy = leaf.Value;
                    return _root.DeleteLeaf(ref mancpy);
                }

                return false;
            }

            public void Clear()
            {
                _root = new Sector(center, range, null, this, 0, -1);
            }

            private bool IsPosValid(Vector3D position)
            {
                var diff = position - center;
                if (Math.Abs(position.X) > range)
                    return false;
                if (Math.Abs(position.Y) > range)
                    return false;
                if (Math.Abs(position.Z) > range)
                    return false;

                return true;
            }

            public int CountLeaves()
            {
                int count = 0;
                _root.GetLeafCountInBranch(ref count);
                return count;
            }

            public class Sector
            {
                public Vector3D center;
                public double halfLength;
                private double quarterLength { get { return halfLength / 2; } }

                private int depth = 0;
                private PositionOctree<T> root;
                private Sector parent;
                private int quadrantNum;

                public Sector[] subSectors;
                public List<Leaf> leaves;

                public Sector(Vector3D center, double halfLength)
                {
                    this.center = center;
                    this.halfLength = halfLength;

                    leaves = new List<Leaf>(8);
                }

                public Sector(Vector3D center, double halfLength, Sector parent, PositionOctree<T> root, int depth, int quadrantNum)
                {
                    this.parent = parent;
                    this.root = root;
                    this.center = center;
                    this.halfLength = halfLength;
                    this.depth = depth;
                    this.quadrantNum = quadrantNum;

                    leaves = new List<Leaf>(8);
                }

                public Leaf? FindExactLeaf(ref Vector3D position)
                {
                    if (subSectors == null)
                    {   // if this is the lowest level node
                        double closestSq = double.MaxValue;
                        int closestI = 0;

                        for (int i = 0; i < leaves.Count; i++)
                        {
                            double dist = Vector3D.DistanceSquared(position, leaves[i].position);

                            if (dist < closestSq)
                            {
                                closestSq = dist;
                                closestI = i;
                            }
                        }

                        if (leaves[closestI].position == position)
                            return leaves[closestI];
                        else
                            return null;
                    }
                    else
                    {
                        return GetSubSector(position).FindExactLeaf(ref position);
                    }
                }

                public Leaf? FindClosestLeaf(ref Vector3D position, List<int> ignoreThese)
                {
                    if (subSectors == null && leaves.Count > 0)
                    {   // if this is the lowest level node

                        if (leaves == null)
                            leaves = new List<Leaf>(8);

                        double closestSq = double.MaxValue;
                        int closestI = 0;

                        for (int i = 0; i < leaves.Count; i++)
                        {
                            double dist = Vector3D.DistanceSquared(position, leaves[i].position);

                            if (dist < closestSq)
                            {
                                closestSq = dist;
                                closestI = i;
                            }
                        }

                        return leaves[closestI];
                    } else
                    {
                        if (ignoreThese == null)
                            ignoreThese = new List<int>();

                        int? sectorInt = GetSubSectorInt(position, ignoreThese);
                        if (!sectorInt.HasValue || subSectors == null)
                        {
                            if (quadrantNum != -1)
                                return null;

                            ignoreThese.Clear();
                            ignoreThese.Add(quadrantNum);
                            return parent.FindClosestLeaf(ref position, ignoreThese);
                        }
                       

                        Sector sector = subSectors[sectorInt.Value];
                        if (sector.subSectors == null)
                        {
                            if (sector.leaves.Count > 0)
                            {
                                ignoreThese.Clear();
                                return sector.FindClosestLeaf(ref position, ignoreThese);
                            }

                            ignoreThese.Add(sectorInt.Value);
                            return FindClosestLeaf(ref position, ignoreThese);
                        }

                        ignoreThese.Clear();
                        return sector.FindClosestLeaf(ref position, ignoreThese);
                    }
                }

                public bool DeleteLeaf(ref Leaf leaf)
                {
                    if (subSectors == null)
                    {   // if this is the lowest level node
                        return leaves.Remove(leaf);
                    } else
                    {
                        return GetSubSector(leaf.position).DeleteLeaf(ref leaf);
                    }
                }

                public bool TryAddLeaf(ref Leaf leaf)
                {
                    if (subSectors == null)
                    {   // if this is the lowest level node
                        if (leaves.Count < 8)
                        {
                            foreach (var subleaf in leaves)
                            {
                                if (subleaf.Equals(leaf))
                                    return false;
                            }

                            leaves.Add(leaf);
                            return true;
                        } else
                        {
                            bool result = true;

                            result &= DivideSector();
                            result &= TryAddLeaf(ref leaf);

                            return result;
                        }
                    } else
                    {
                        Sector sector = GetSubSector(leaf.position);
                        return sector.TryAddLeaf(ref leaf);
                    }
                }

                public void GetLeafCountInBranch(ref int count)
                {
                    if (subSectors != null)
                    {
                        foreach (var sub in subSectors)
                            sub.GetLeafCountInBranch(ref count);
                    }
                    else
                        count += leaves.Count;
                }

                private int? GetSubSectorInt(Vector3D position, List<int> ignoreSectors = null)
                {
                    Vector3D dfc = position - center;
                    int correctSector = 0;

                    if (dfc.Z > 0)
                    {
                        if (dfc.Y > 0)
                        {
                            if (dfc.X > 0)
                                correctSector = 0;
                            else
                                correctSector = 1;
                        }

                        if (dfc.X > 0)
                            correctSector = 2;
                        else
                            correctSector = 3;
                    } else
                    {
                        if (dfc.Y > 0)
                        {
                            if (dfc.X > 0)
                                correctSector = 4;
                            else
                                correctSector = 5;
                        }

                        if (dfc.X > 0)
                            correctSector = 6;
                        else
                            correctSector = 7;
                    }

                    if (ignoreSectors == null || !ignoreSectors.Contains(correctSector))
                        return correctSector;

                    double closestSq = double.MaxValue;

                    for (int i = 0; i < subSectors.Length; i++)
                    {
                        if (ignoreSectors.Contains(i))
                            continue;

                        double newDist = Vector3D.DistanceSquared(position, subSectors[i].center);

                        if (newDist < closestSq)
                        {
                            closestSq = newDist;
                            correctSector = i;
                        }
                    }

                    if (!ignoreSectors.Contains(correctSector))
                        return correctSector;

                    return null;
                }

                private Sector GetSubSector(Vector3D position)
                {
                    Vector3D dfc = position - center;
                    int correctSector = 0;

                    if (dfc.Z > 0)
                    {
                        if (dfc.Y > 0)
                        {
                            if (dfc.X > 0)
                                correctSector = 0;
                            else
                                correctSector = 1;
                        }

                        if (dfc.X > 0)
                            correctSector = 2;
                        else
                            correctSector = 3;
                    }
                    else
                    {
                        if (dfc.Y > 0)
                        {
                            if (dfc.X > 0)
                                correctSector = 4;
                            else
                                correctSector = 5;
                        }

                        if (dfc.X > 0)
                            correctSector = 6;
                        else
                            correctSector = 7;
                    }

                    return subSectors[correctSector];
                }

                private bool DivideSector()
                {
                    bool success = true;

                    subSectors = new Sector[8];

                    // create all the new sectors offset from the center by the quarter length
                    #region create
                    int subDepth = depth + 1;

                    if (subDepth >= root.maxDepth)
                        throw new Exception("depth Exceeded maximum tree depth!");

                    subSectors[0] = new Sector(new Vector3D(center.X + quarterLength, center.Y + quarterLength, center.Z + quarterLength), quarterLength, this, root, subDepth, 0);

                    subSectors[1] = new Sector(new Vector3D(center.X - quarterLength, center.Y + quarterLength, center.Z + quarterLength), quarterLength, this, root, subDepth, 1);

                    subSectors[2] = new Sector(new Vector3D(center.X + quarterLength, center.Y - quarterLength, center.Z + quarterLength), quarterLength, this, root, subDepth, 2);

                    subSectors[3] = new Sector(new Vector3D(center.X - quarterLength, center.Y - quarterLength, center.Z + quarterLength), quarterLength, this, root, subDepth, 3);

                    subSectors[4] = new Sector(new Vector3D(center.X + quarterLength, center.Y + quarterLength, center.Z - quarterLength), quarterLength, this, root, subDepth, 4);

                    subSectors[5] = new Sector(new Vector3D(center.X - quarterLength, center.Y + quarterLength, center.Z - quarterLength), quarterLength, this, root, subDepth, 5);

                    subSectors[6] = new Sector(new Vector3D(center.X + quarterLength, center.Y - quarterLength, center.Z - quarterLength), quarterLength, this, root, subDepth, 6);

                    subSectors[7] = new Sector(new Vector3D(center.X - quarterLength, center.Y - quarterLength, center.Z - quarterLength), quarterLength, this, root, subDepth, 7);
                    #endregion

                    // actually move the leaves
                    foreach (var leaf in leaves)
                    {
                        var mancpy = leaf;
                        success &= TryAddLeaf(ref mancpy);
                    }

                    // clear leaves from node this if success
                    if (success)
                        leaves = null;

                    return success;
                }
            }

            public struct Leaf : IEquatable<Leaf>
            {
                public Vector3D position;
                public T payload;

                public Leaf(Vector3D position, T payload)
                {
                    this.position = position;
                    this.payload = payload;
                }

                public bool Equals(Leaf other)
                {
                    return position == other.position;
                }

                public override bool Equals(object obj)
                {
                    if (!(obj is Leaf))
                        return false;

                    return Equals((Leaf)obj);
                }

                public override int GetHashCode()
                {
                    return 1206833562 + EqualityComparer<Vector3D>.Default.GetHashCode(position);
                }
            }
        }
    }
}

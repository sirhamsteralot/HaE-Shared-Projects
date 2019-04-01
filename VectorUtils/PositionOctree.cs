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
                _root = new Sector(center, range);
            }

            public PositionOctree()
            {
                maxDepth = 50;
                _root = new Sector(Vector3D.Zero, double.MaxValue);
            }

            public Leaf FindClosestLeaf(Vector3D position)
            {
                return _root.FindClosestLeaf(ref position);
            }

            public bool TryAddLeaf(Vector3D position, T payload)
            {
                var diff = position - center;
                if (Math.Abs(position.X) > range)
                    return false;
                if (Math.Abs(position.Y) > range)
                    return false;
                if (Math.Abs(position.Z) > range)
                    return false;

                var leaf = new Leaf(position, payload);

                return _root.TryAddLeaf(ref leaf);
            }

            public bool TryDeleteLeafAt(Vector3D position)
            {
                var leaf = FindClosestLeaf(position);
                return _root.DeleteLeaf(ref leaf);
            }

            public void Clear()
            {
                _root = new Sector(center, range, this, 0);
            }

            public class Sector
            {
                public Vector3D center;
                public double halfLength;
                private double quarterLength { get { return halfLength / 2; } }

                private int depth = 0;
                private PositionOctree<T> root;

                public Sector[] subSectors;
                public List<Leaf> leaves;

                public Sector(Vector3D center, double halfLength)
                {
                    this.center = center;
                    this.halfLength = halfLength;

                    leaves = new List<Leaf>(8);
                }

                public Sector(Vector3D center, double halfLength, PositionOctree<T> root, int depth)
                {
                    this.root = root;
                    this.center = center;
                    this.halfLength = halfLength;
                    this.depth = depth;

                    leaves = new List<Leaf>(8);
                }

                public Leaf FindClosestLeaf(ref Vector3D position)
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

                        return leaves[closestI];
                    } else
                    {
                        return GetSubSector(position).FindClosestLeaf(ref position);
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
                            result &= GetSubSector(leaf.position).TryAddLeaf(ref leaf);

                            return result;
                        }
                    } else
                    {
                        Sector sector = GetSubSector(leaf.position);
                        return sector.TryAddLeaf(ref leaf);
                    }
                }

                private Sector GetSubSector(Vector3D position)
                {
                    Vector3D dfc = position - center;

                    if (dfc.Z > 0)
                    {
                        if (dfc.Y > 0)
                        {
                            if (dfc.X > 0)
                            {
                                return subSectors[0];
                            }

                            return subSectors[1];
                        }

                        if (dfc.X > 0)
                        {
                            return subSectors[2];
                        }

                        return subSectors[3];
                    }

                    if (dfc.Y > 0)
                    {
                        if (dfc.X > 0)
                        {
                            return subSectors[4];
                        }

                        return subSectors[5];
                    }

                    if (dfc.X > 0)
                    {
                        return subSectors[6];
                    }

                    return subSectors[7];
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

                    subSectors[0] = new Sector(new Vector3D(center.X + quarterLength, center.Y + quarterLength, center.Z + quarterLength), quarterLength, root, subDepth);

                    subSectors[1] = new Sector(new Vector3D(center.X - quarterLength, center.Y + quarterLength, center.Z + quarterLength), quarterLength, root, subDepth);

                    subSectors[2] = new Sector(new Vector3D(center.X + quarterLength, center.Y - quarterLength, center.Z + quarterLength), quarterLength, root, subDepth);

                    subSectors[3] = new Sector(new Vector3D(center.X - quarterLength, center.Y - quarterLength, center.Z + quarterLength), quarterLength, root, subDepth);

                    subSectors[4] = new Sector(new Vector3D(center.X + quarterLength, center.Y + quarterLength, center.Z - quarterLength), quarterLength, root, subDepth);

                    subSectors[5] = new Sector(new Vector3D(center.X - quarterLength, center.Y + quarterLength, center.Z - quarterLength), quarterLength, root, subDepth);

                    subSectors[6] = new Sector(new Vector3D(center.X + quarterLength, center.Y - quarterLength, center.Z - quarterLength), quarterLength, root, subDepth);

                    subSectors[7] = new Sector(new Vector3D(center.X - quarterLength, center.Y - quarterLength, center.Z - quarterLength), quarterLength, root, subDepth);
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

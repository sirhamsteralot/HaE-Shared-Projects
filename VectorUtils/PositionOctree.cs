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

            public void Clear()
            {
            }

            public struct Sector
            {
                public Vector3D center;
                public double halfLength;
                private double quarterLength { get { return halfLength / 2; } }


                public List<Sector> subSectors;
                public List<Leaf> leaves;

                public Sector(Vector3D center, double halfLength) : this()
                {
                    this.center = center;
                    this.halfLength = halfLength;
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

                public void DeleteLeaf(ref Leaf leaf)
                {
                    if (subSectors == null)
                    {   // if this is the lowest level node
                        leaves.Remove(leaf);
                    } else
                    {
                        GetSubSector(leaf.position).DeleteLeaf(ref leaf);
                    }
                }

                public bool AddLeaf(ref Leaf leaf)
                {
                    if (subSectors == null)
                    {   // if this is the lowest level node
                        if (leaves == null)
                            leaves = new List<Leaf>();

                        if (leaves.Count < 8)
                        {
                            foreach (var subleaf in leaves)
                            {
                                if (subleaf.Equals(leaf))
                                    return false;
                            }

                            leaves.Add(leaf);
                        } else
                        {
                            DivideSector();
                            return AddLeaf(ref leaf);
                        }

                        return false;
                    } else
                    {
                        return GetSubSector(leaf.position).AddLeaf(ref leaf);
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
                    else
                    {
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
                }

                private void DivideSector()
                {
                    var divided = new List<Sector>();

                    // create all the new sectors offset from the center by the quarter length
                    #region create
                    Vector3D tempvec = center;
                    tempvec.X += quarterLength; tempvec.Y += quarterLength; tempvec.Z += quarterLength;
                    divided.Add(new Sector(tempvec, quarterLength));

                    tempvec = center;
                    tempvec.X -= quarterLength; tempvec.Y += quarterLength; tempvec.Z += quarterLength;
                    divided.Add(new Sector(tempvec, quarterLength));

                    tempvec = center;
                    tempvec.X += quarterLength; tempvec.Y -= quarterLength; tempvec.Z += quarterLength;
                    divided.Add(new Sector(tempvec, quarterLength));

                    tempvec = center;
                    tempvec.X -= quarterLength; tempvec.Y -= quarterLength; tempvec.Z += quarterLength;
                    divided.Add(new Sector(tempvec, quarterLength));

                    tempvec = center;
                    tempvec.X += quarterLength; tempvec.Y += quarterLength; tempvec.Z -= quarterLength;
                    divided.Add(new Sector(tempvec, quarterLength));

                    tempvec = center;
                    tempvec.X -= quarterLength; tempvec.Y += quarterLength; tempvec.Z -= quarterLength;
                    divided.Add(new Sector(tempvec, quarterLength));

                    tempvec = center;
                    tempvec.X += quarterLength; tempvec.Y -= quarterLength; tempvec.Z -= quarterLength;
                    divided.Add(new Sector(tempvec, quarterLength));

                    tempvec = center;
                    tempvec.X -= quarterLength; tempvec.Y -= quarterLength; tempvec.Z -= quarterLength;
                    divided.Add(new Sector(tempvec, quarterLength));
                    #endregion

                    // make the new subsectors active and move over all the leaves before clearing them from this node
                    subSectors = divided;
                    var tempBranch = leaves;
                    leaves = null;

                    // actually move the leaves
                    foreach (var leaf in leaves)
                    {
                        var mancpy = leaf;
                        AddLeaf(ref mancpy);
                    }
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

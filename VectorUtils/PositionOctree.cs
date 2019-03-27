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

                public void AddLeaf(Leaf leaf)
                {
                    if (subSectors == null)
                    {   // if this is the lowest level node
                        if (leaves == null)
                            leaves = new List<Leaf>();

                        if (leaves.Count < 8)
                        {
                            leaves.Add(leaf);
                        } else
                        {
                            DivideSector();
                            AddLeaf(leaf);
                        }

                        return;
                    } else
                    {   // this isnt the lowest level node
                        Vector3D dfc = leaf.position - center;

                        if (dfc.Z > 0)
                        {
                            if (dfc.Y > 0)
                            {
                                if (dfc.X > 0)
                                {
                                    subSectors[0].AddLeaf(leaf);
                                    return;
                                }

                                subSectors[1].AddLeaf(leaf);
                                return;
                            }

                            if (dfc.X > 0)
                            {
                                subSectors[2].AddLeaf(leaf);
                                return;
                            }

                            subSectors[3].AddLeaf(leaf);
                            return;
                        }
                            else
                        {
                            if (dfc.Y > 0)
                            {
                                if (dfc.X > 0)
                                {
                                    subSectors[4].AddLeaf(leaf);
                                    return;
                                }

                                subSectors[5].AddLeaf(leaf);
                                return;
                            }

                            if (dfc.X > 0)
                            {
                                subSectors[6].AddLeaf(leaf);
                                return;
                            }

                            subSectors[7].AddLeaf(leaf);
                            return;
                        }
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
                        AddLeaf(leaf);
                    }
                }
            }

            public struct Leaf
            {
                public Vector3D position;
                public T payload;

                public Leaf(Vector3D position, T payload)
                {
                    this.position = position;
                    this.payload = payload;
                }
            }
        }
    }
}

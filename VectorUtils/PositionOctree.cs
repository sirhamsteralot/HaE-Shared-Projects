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
        public class PositionOctree
        {
            private Node topNode;

            public PositionOctree()
            {
                topNode = new Node(Vector3D.Zero, null);
            }

            public void AddPositionNode(Vector3D position)
            {
                topNode.AddPosition(position);
            }

            public bool DeleteNode(Node node)
            {
                if (node.Equals(topNode))
                    return false;

                node.DeleteNode();
                return true;
            }

            public Node FindClosestNode(Vector3D position)
            {
                return topNode.FindClosestNode(position);
            }

            public struct Node
            {
                public Vector3D position;

                public object parentNode;
                public List<Node> subNodes;

                public Node(Vector3D position, object parentNode) : this()
                {
                    this.parentNode = parentNode;
                    this.position = position;
                }

                public void AddPosition(Vector3D position)
                {
                    if (subNodes.Count <= 8)
                    {
                        subNodes.Add(new Node(position, this));
                    } else
                    {
                        FindClosestNode(position).AddPosition(position);
                    }
                }

                public Node FindClosestNode(Vector3D position)
                {
                    if (subNodes.Count < 1)
                        return this;

                    int closest = 0;
                    double closestDistSq = double.MaxValue;

                    for (int i = 0; i < subNodes.Count; i++)
                    {
                        double distSq = Vector3D.DistanceSquared(position, subNodes[i].position);

                        if (distSq < closestDistSq)
                        {
                            closest = i;
                            closestDistSq = distSq;
                        }
                    }

                    Node closestNode = subNodes[closest];

                    if (closestNode.subNodes.Count > 0)
                    {
                        return closestNode.FindClosestNode(position);
                    }

                    return closestNode;
                }

                public void DeleteNode()
                {
                    if (parentNode is Node)
                    {
                        Node parent = (Node)parentNode;
                        parent.DeleteSubNode(this);
                    }

                    
                }

                public void DeleteSubNode(Node node)
                {
                    bool deleted = false;

                    for (int i = 0; i < subNodes.Count; i++)
                    {
                        if (subNodes[i].Equals(node))
                        {
                            subNodes.RemoveAt(i);
                            deleted = true;
                            break;
                        }
                    }

                    if (node.subNodes.Count > 0 && deleted)
                    {
                        subNodes.Add(node.subNodes[0]);
                    }
                }

                public override bool Equals(object obj)
                {
                    if (!(obj is Node))
                        return false;

                    return position.Equals(((Node)obj).position);
                }

                public override int GetHashCode()
                {
                    return position.GetHashCode();
                }
            }
        }
    }
}

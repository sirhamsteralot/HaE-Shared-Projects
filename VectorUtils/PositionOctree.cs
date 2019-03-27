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
                topNode = new Node(Vector3D.Zero, this, null);
            }

            public void AddPositionNode(Vector3D position, object payload = null)
            {
                topNode.AddPosition(position, payload);
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
                Node closestCache = new Node();
                double closestSqCache = double.MaxValue;
                return topNode.FindClosestNode(position, ref closestCache, ref closestSqCache);
            }

            public void Clear()
            {
                topNode = new Node(Vector3D.Zero, null, this, null);
            }

            public List<Node> GetNodeList()
            {
                List<Node> workList = new List<Node>();
                workList.Clear();
                workList.AddRange(GetNodeList(ref topNode, ref workList));
                return workList;
            }
            private List<Node> GetNodeList(ref Node sourceNode, ref List<Node> workList)
            {
                foreach (var node in sourceNode.subNodes)
                {
                    var manCpy = node;
                    workList.AddRange(GetNodeList(ref manCpy, ref workList));
                }

                return sourceNode.subNodes;
            }

            public struct Node
            {
                public Vector3D position;
                public object payload;

                public object parentNode;
                public PositionOctree root;
                public List<Node> subNodes;

                public Node(Vector3D position, object parentNode, PositionOctree root, object payload = null) : this()
                {
                    this.parentNode = parentNode;
                    this.position = position;
                    this.root = root;
                    this.payload = payload;

                    subNodes = new List<Node>();
                }

                public void AddPosition(Vector3D position, object payload = null)
                {
                    Node closestCache = new Node();
                    double closestSqCache = double.MaxValue;

                    if (subNodes.Count < 8)
                    {
                        subNodes.Add(new Node(position, this, root, payload));
                    } else
                    {
                        FindClosestFreeNode(position, ref closestCache, ref closestSqCache).AddPosition(position);
                    }
                }

                public void AddNode(Node node)
                {
                    Node closestCache = new Node();
                    double closestSqCache = double.MaxValue;

                    if (subNodes.Count < 8)
                    {
                        subNodes.Add(node);
                    }
                    else
                    {
                        FindClosestFreeNode(node.position, ref closestCache, ref closestSqCache).AddNode(node);
                    }
                }

                public Node FindClosestNode(Vector3D position, ref Node closestNode, ref double closestDistSq)
                {
                    if (subNodes.Count < 1)
                        return this;

                    double closestDistSqNode = double.MaxValue;
                    for (int i = 0; i < subNodes.Count; i++)
                    {
                        double distSq = Vector3D.DistanceSquared(position, subNodes[i].position);

                        if (distSq < closestDistSqNode)
                        {
                            closestNode = subNodes[i];
                            closestDistSqNode = distSq;
                        }
                    }

                    closestDistSq = closestDistSqNode;

                    if (closestNode.subNodes.Count > 0)
                    {
                        return closestNode.FindClosestNode(position, ref closestNode, ref closestDistSq);
                    }

                    return closestNode;
                }

                public Node FindClosestFreeNode(Vector3D position, ref Node closestNode, ref double closestDistSq)
                {
                    if (subNodes.Count < 1)
                        return this;

                    int closest = 0;
                    closestDistSq = double.MaxValue;

                    for (int i = 0; i < subNodes.Count; i++)
                    {
                        double distSq = Vector3D.DistanceSquared(position, subNodes[i].position);

                        if (distSq < closestDistSq)
                        {
                            closest = i;
                            closestDistSq = distSq;
                        }
                    }

                    closestNode = subNodes[closest];

                    if (closestNode.subNodes.Count > 7)
                    {
                        return closestNode.FindClosestFreeNode(position, ref closestNode, ref closestDistSq);
                    }

                    return closestNode;
                }

                public bool DeleteNode()
                {
                    if (parentNode is Node)
                    {
                        Node parent = (Node)parentNode;
                        return parent.DeleteSubNode(this);
                    }
                    return false;
                }

                public bool DeleteSubNode(Node node)
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

                    if (deleted)
                    {
                        foreach (var subNode in node.subNodes)
                        {
                            root.AddPositionNode(subNode.position, subNode.payload);
                        }
                    }

                    return deleted;
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

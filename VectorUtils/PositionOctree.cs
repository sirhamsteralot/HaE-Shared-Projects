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

            public void AddPositionNode(Vector3D position)
            {
                topNode.AddPosition(position);
            }

            public Node FindClosestNode(Vector3D position)
            {
                return topNode.FindClosestNode(position);
            }

            public struct Node
            {
                Vector3D position;

                public List<Node> subNodes;

                public Node(Vector3D position) : this()
                {
                    this.position = position;
                }

                public void AddPosition(Vector3D position)
                {
                    if (subNodes.Count <= 8)
                    {
                        subNodes.Add(new Node(position));
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
            }
        }
    }
}

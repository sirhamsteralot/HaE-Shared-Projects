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
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
	partial class Program
	{
        public class AdvThrustControl
	    {
            private IMyShipController controller;

            private Dictionary<Vector3D, List<IMyThrust>> sortedThrusters = new Dictionary<Vector3D, List<IMyThrust>>();
            private Dictionary<Vector3D, int> thrustersPerDirection = new Dictionary<Vector3D, int>();

            public AdvThrustControl(IMyShipController controller, List<IMyThrust> thrusters, Dictionary<Vector3D, int> thrustersPerDirection)
            {
                this.controller = controller;
                this.thrustersPerDirection = thrustersPerDirection;

                sortedThrusters = ThrustUtils.SortThrustersByDirection(thrusters, controller);
                CountThrustersPerDirection();
            }

            public void CountThrustersPerDirection()
            {
                foreach (var thrustInDir in sortedThrusters.Keys)
                {
                    var thrusterList = sortedThrusters[thrustInDir];

                    thrustersPerDirection[thrustInDir] = thrusterList.Count;
                }
            }

            //NOT WORKING
            public void ThrustInDirection(Vector3D direction, double newtons)
            {
                Vector3D localDirection = -Vector3D.TransformNormal(direction, controller.WorldMatrix);

                int highestThrustInDirection = 0;
                int lowestThrustInDirection = int.MaxValue;

                foreach (var thrustInDir in sortedThrusters.Keys)
                {
                    if (thrustInDir.Dot(localDirection) < 0)
                        continue;

                    var thrusterList = sortedThrusters[thrustInDir];

                    int thrusterListCount = thrusterList.Count;
                    if (highestThrustInDirection < thrusterListCount)
                        highestThrustInDirection = thrusterListCount;
                    else if (lowestThrustInDirection > thrusterListCount)
                        lowestThrustInDirection = thrusterListCount;
                }

                if (lowestThrustInDirection == 0)
                    return;

                foreach (var thrustInDir in sortedThrusters.Keys)
                {
                    if (thrustInDir.Dot(localDirection) < 0)
                        continue;

                    var thrusterList = sortedThrusters[thrustInDir];

                    double countMultiplier = (double)lowestThrustInDirection / (double)thrusterList.Count;
                    double splitMultiplier = 1 - VectorUtils.GetProjectionScalar(localDirection, thrustInDir);

                    double newtonActivation = countMultiplier * splitMultiplier * newtons;

                    foreach (var thruster in thrusterList)
                    {
                        thruster.ThrustOverride = (float)newtonActivation;
                    }
                }
            }
        }
	}
}

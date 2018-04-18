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
        public class Autopilot_Module
	    {
            public PID_Controller.PIDSettings PIDControlSettings = new PID_Controller.PIDSettings
            {
                PGain = 21,
                IntegralGain = 0,
                DerivativeGain = -1,
            };

            private PID_Controller PIDControl;
            private DateTime lastUpdateTime;
            private Program P;

            private List<IMyThrust> allThrusters = new List<IMyThrust>();
            private List<IMyGyro> gyros = new List<IMyGyro>();
            private Dictionary<Vector3D, List<IMyThrust>> sortedThrusters = new Dictionary<Vector3D, List<IMyThrust>>();
            private Dictionary<Vector3D, int> thrustersPerDirection = new Dictionary<Vector3D, int>();

            private IMyShipController controller;

            public Autopilot_Module(GridTerminalSystemUtils GTS, IMyShipController controller, Program P)
            {
                GTS.GridTerminalSystem.GetBlocksOfType(allThrusters);
                GTS.GridTerminalSystem.GetBlocksOfType(gyros);

                this.controller = controller;
                this.P = P;

                sortedThrusters = ThrustUtils.SortThrustersByDirection(allThrusters, controller);
                CountThrustersPerDirection();

                PIDControl = new PID_Controller(PIDControlSettings.PGain, PIDControlSettings.IntegralGain, PIDControlSettings.DerivativeGain);
            }

            public void TravelToPosition(Vector3D position)
            {

            }

            public void PointInDirection(Vector3D direction, Vector3D alignUpTo)
            {
                var timespan = DateTime.Now - lastUpdateTime;
                double error = Vector3D.Dot(controller.WorldMatrix.Right, direction);
                double multiplier = MyMath.Clamp( (float)Math.Abs(PIDControl.NextValue(error, timespan.TotalSeconds)), 0.1f, 100);

                GyroUtils.PointInDirection(gyros, controller, direction, alignUpTo, multiplier);
                lastUpdateTime = DateTime.Now;
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

                P.Echo($"lowestThrustDirection: {lowestThrustInDirection}");
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

                    P.Echo($"newtonActivation: {newtonActivation}");

                    foreach (var thruster in thrusterList)
                    {
                        thruster.ThrustOverride = (float)newtonActivation;
                    }
                }
            }

            private void CountThrustersPerDirection()
            {
                foreach (var thrustInDir in sortedThrusters.Keys)
                {
                    var thrusterList = sortedThrusters[thrustInDir];

                    thrustersPerDirection[thrustInDir] = thrusterList.Count;
                }
            }

            public void ThrustToVelocity(Vector3D velocity, Vector3D compensateForGravity)
            {

            }
        }
	}
}

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
            

            private List<IMyThrust> allThrusters = new List<IMyThrust>();
            private List<IMyGyro> gyros = new List<IMyGyro>();
            private Dictionary<Vector3D, List<IMyThrust>> sortedThrusters = new Dictionary<Vector3D, List<IMyThrust>>();
            private Dictionary<Vector3D, double> sortedMultipliers = new Dictionary<Vector3D, double>();

            private IMyShipController controller;

            public Autopilot_Module(GridTerminalSystemUtils GTS, IMyShipController controller)
            {
                GTS.GridTerminalSystem.GetBlocksOfType(allThrusters);
                GTS.GridTerminalSystem.GetBlocksOfType(gyros);

                this.controller = controller;

                sortedThrusters = ThrustUtils.SortThrustersByDirection(allThrusters);

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

            public void ThrustInDirection(Vector3D direction, Vector3D compensateForGravity)
            {
                Vector3D directionToThrust = direction;

                if (compensateForGravity != Vector3D.Zero)
                {
                    Vector3D gravity = -compensateForGravity;

                    foreach (var thrustInDir in sortedThrusters)
                    {

                    }
                }

                ThrustUtils.SetThrustBasedDot(allThrusters, directionToThrust);
            }
        }
	}
}

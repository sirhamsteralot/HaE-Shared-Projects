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
            public PID_Controller.PIDSettings PIDsettings = new PID_Controller.PIDSettings
            {
                PGain = 21,
                IntegralGain = 0.001,
                DerivativeGain = 1,
            };

            private PID_Controller PIDControl;
            private DateTime lastUpdateTime;

            private ThrustersInDirection sortedThrusters;
            

            private List<IMyThrust> allThrusters = new List<IMyThrust>();
            private List<IMyGyro> gyros = new List<IMyGyro>();

            private IMyShipController controller;

            public Autopilot_Module(GridTerminalSystemUtils GTS, IMyShipController controller)
            {
                GTS.GridTerminalSystem.GetBlocksOfType(allThrusters);
                GTS.GridTerminalSystem.GetBlocksOfType(gyros);

                this.controller = controller;

                sortedThrusters = ThrustUtils.SortThrustByDirection(allThrusters, controller);
                PIDControl = new PID_Controller(PIDsettings.PGain, PIDsettings.IntegralGain, PIDsettings.DerivativeGain);
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
        }
	}
}

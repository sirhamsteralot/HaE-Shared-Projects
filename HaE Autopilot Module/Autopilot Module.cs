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
            private PID_Controller PIDControl;
            private IngameTime ingameTime;

            private List<IMyThrust> allThrusters = new List<IMyThrust>();
            private List<IMyGyro> gyros = new List<IMyGyro>();

            private IMyShipController controller;

            private AdvThrustControl thrustControl;
            private AdvGyroControl gyroControl;

            public Autopilot_Module(GridTerminalSystemUtils GTS, IMyShipController controller, IngameTime ingameTime, PID_Controller.PIDSettings GyroPidSettings)
            {
                GTS.GridTerminalSystem.GetBlocksOfType(allThrusters);
                GTS.GridTerminalSystem.GetBlocksOfType(gyros);

                this.controller = controller;
                this.ingameTime = ingameTime;

                gyroControl = new AdvGyroControl(GyroPidSettings, ingameTime);
            }

            public void TravelToPosition(Vector3D position)
            {

            }

            public void ThrustToVelocity(Vector3D velocity, Vector3D compensateForGravity)
            {

            }
        }
	}
}

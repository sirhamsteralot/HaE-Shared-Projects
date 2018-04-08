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
            private ThrustersInDirection sortedThrusters;
            private List<IMyThrust> allThrusters;

            private IMyShipController controller;

            public Autopilot_Module(List<IMyThrust> allThrusters, IMyShipController controller)
            {
                this.allThrusters = allThrusters;
                this.controller = controller;

                sortedThrusters = ThrustUtils.SortThrustByDirection(allThrusters, controller);
            }

            public void TravelToPosition()
            {

            }
        }
	}
}

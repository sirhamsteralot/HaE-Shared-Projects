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
        public class AdvancedSimTargeting
	    {
            private Vector3D firingDirection;

            private IMyShipController control;

            private MyDetectedEntityInfo target;

            private double speedlimit;

            public IEnumerator<bool> RunSimulation()
            {

            }


            public class ProjectileInfo
            {
                private double projectileMaxSpeed;
                private Vector3D projectileAcceleration;
                private Vector3D currentLocation;
                private Vector3D currentVelocity;
            }
	    }
	}
}

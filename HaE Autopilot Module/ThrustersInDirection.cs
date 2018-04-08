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
        public class ThrustersInDirection : IEnumerable
	    {
            List<IMyThrust>[] thrusters;

            public int minThrustInDirection = 0;
            public int Count => thrusters.Length;

            public List<IMyThrust> this[int i]
            {
                get { return thrusters[i]; }
                set { thrusters[i] = value; }
            }

            public List<IMyThrust> this[Base6Directions.Direction i]
            {
                get { return thrusters[(int)i]; }
                set { thrusters[(int)i] = value; }
            }

            public ThrustersInDirection(List<IMyThrust>[] thrusters, int minThrustInDirection)
            {
                this.thrusters = thrusters;
                this.minThrustInDirection = minThrustInDirection;
            }

            public IEnumerator GetEnumerator()
            {
                return thrusters.GetEnumerator();
            }
        }
	}
}

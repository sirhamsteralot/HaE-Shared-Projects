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
        public class IngameTime
	    {
            private const double TICKTIME = 16.0;
            private long tickCounter;

            public TimeSpan Time { get { return TicksToTime(); }}

            public void Tick()
            {
                tickCounter++;
            }

            public void Tick(int ticks)
            {
                tickCounter += ticks;
            }

            public void Tick(double timeSinceLastRun)
            {
                Tick((int)Math.Round(timeSinceLastRun / TICKTIME));
            }

            public void Tick(TimeSpan timeSinceLastRun)
            {
                Tick(timeSinceLastRun.TotalMilliseconds / TICKTIME);
            }

            public void Tick(IMyGridProgramRuntimeInfo runtime)
            {
                Tick(runtime.TimeSinceLastRun);
            }

            private TimeSpan TicksToTime()
            {
                return TimeSpan.FromMilliseconds(TicksToMillis());
            }

            private double TicksToMillis()
            {
                return tickCounter * TICKTIME;
            }
	    }
	}
}

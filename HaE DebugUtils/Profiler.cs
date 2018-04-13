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
        public class Profiler
	    {
            public Action OnProfilingFinished;

            StringBuilder sb = new StringBuilder();
            int ticksToRun;
            int currentTicks;
            bool isProfiling;

            public Profiler(int ticksToRun, bool autoStartProfiling = false)
            {
                this.ticksToRun = ticksToRun;
                this.isProfiling = autoStartProfiling;
            }

            public void StartProfiling()
            {
                isProfiling = true;
            }

            public void AddValue(double RuntimeMs)
            {
                if (!isProfiling)
                    return;

                if (currentTicks++ > ticksToRun)
                {
                    OnProfilingFinished?.Invoke();
                    return;
                }

                sb.Append(RuntimeMs.ToString() + "\n");
            }

            public void DumpValues(IMyTerminalBlock block)
            {
                block.CustomData = sb.ToString();
            }
	    }
	}
}

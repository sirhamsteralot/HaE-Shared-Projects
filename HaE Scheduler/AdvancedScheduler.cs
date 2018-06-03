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
        public class AdvancedScheduler : Scheduler
	    {
            int instructionMargin;
            int maxInstructionCount;

            IMyGridProgramRuntimeInfo runtimeInfo;


	        public AdvancedScheduler(int maxRunsPerTick, int instructionMargin, IMyGridProgramRuntimeInfo runtimeInfo) : base(maxRunsPerTick)
            {
                this.instructionMargin = instructionMargin;
                this.runtimeInfo = runtimeInfo;

                maxInstructionCount = runtimeInfo.MaxInstructionCount - instructionMargin;
            }

            public new bool Main()
            {
                int runcounter = 0;

                while(runtimeInfo.CurrentInstructionCount <= maxInstructionCount && runcounter++ < runsPerTick)
                {
                    if (queue.Count == 0)
                        return false;

                    var current = queue.First();

                    if (!current.MoveNext())
                    {
                        queue.Dequeue().Dispose();
                        current.Callback?.Invoke();
                    }
                }

                return true;
            }
	    }
	}
}

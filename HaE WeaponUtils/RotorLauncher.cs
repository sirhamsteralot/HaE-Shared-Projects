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
        public class RotorLauncher
	    {
            private IMyMotorStator[] propellingRotorArray;
            public IMyMotorStator launchRotor;

            IngameTime ingameTime;
            TimeSpan previousFire;
            private double timeoutS;


            private Scheduler internalScheduler = new Scheduler();

            public RotorLauncher(IMyMotorStator baseRotor, IngameTime ingameTime, double timeoutS)
            {
                var propellingRotors = new List<IMyMotorStator>();

                this.ingameTime = ingameTime;
                this.timeoutS = timeoutS;

                var currentrotor = baseRotor;

                while(currentrotor != null)
                {
                    propellingRotors.Add(currentrotor);
                    
                    currentrotor = Selector(currentrotor.TopGrid);
                }

                launchRotor = propellingRotors[propellingRotors.Count - 1];
                propellingRotorArray = new IMyMotorStator[propellingRotors.Count];
                for(int i = 0; i < propellingRotors.Count; i++)
                {
                    propellingRotorArray[i] = propellingRotors[i];
                }
                propellingRotors = null;
            }

            public void Tick()
            {
                internalScheduler.Main();
            }

            public void Salvo(int amount)
            {
                if ((ingameTime.Time - previousFire).TotalSeconds < timeoutS || internalScheduler.QueueCount > 0)
                    return;

                for (int i = 0; i < amount; i++)
                    internalScheduler.AddTask(LaunchSequence());

                previousFire = ingameTime.Time;
            }

            public IEnumerator<bool> LaunchSequence()
            {
                foreach (var propellingRotor in propellingRotorArray)
                {
                    propellingRotor.Displacement = 20;
                }

                yield return true;
                
                launchRotor.ApplyAction("Add Top Part");

                foreach (var propellingRotor in propellingRotorArray)
                {
                    propellingRotor.Displacement = -40;
                }
                
                yield return true;

                launchRotor.Detach();

                yield return true;
            }

            public static IMyMotorStator Selector(IMyCubeGrid cubegrid)
            {
                if (cubegrid == null)
                    return null;

                for (int x = cubegrid.Min.X; x <= cubegrid.Max.X; x++)
                {
                    for (int y = cubegrid.Min.Y; y <= cubegrid.Max.Y; y++)
                    {
                        for (int z = cubegrid.Min.Z; z <= cubegrid.Max.Z; z++)
                        {
                            var vec = new Vector3I(x, y, z);
                            var slimblock = cubegrid.GetCubeBlock(vec);
                            IMyMotorStator stator = null;

                            if (slimblock != null)
                                stator = slimblock.FatBlock as IMyMotorStator;

                            if (stator != null && !stator.CustomName.Contains("[Ignore]"))
                                return stator;
                        }
                    }
                }

                return null;
            }
        }
    }
}

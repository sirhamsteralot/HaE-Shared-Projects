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
            public List<IMyMotorStator> propellingRotors = new List<IMyMotorStator>();
            public IMyMotorStator launchRotor;

            public RotorLauncher(List<IMyMotorStator> propellingRotors, IMyMotorStator launchRotor)
            {
                this.propellingRotors = propellingRotors;
                this.launchRotor = launchRotor;
            }

            public IEnumerator<bool> LaunchSequence()
            {

                foreach (var propellingRotor in propellingRotors)
                {
                    propellingRotor.Displacement = 20;
                }

                yield return true;
                
                launchRotor.ApplyAction("Add Top Part");

                foreach (var propellingRotor in propellingRotors)
                {
                    propellingRotor.Displacement = -40;
                }
                
                yield return true;

                launchRotor.Detach();
            }
        }
    }
}

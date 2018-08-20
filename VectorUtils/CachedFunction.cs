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
        public class CachedFunction<T,TResult>
        {
            public Func<T, TResult> mathFunction;
            public Dictionary<T, TResult> cachedValues = new Dictionary<T, TResult>();

            public CachedFunction(Func<T, TResult> mathFunction)
            {
                this.mathFunction = mathFunction;
            }

            public TResult Execute(T argument)
            {
                if (cachedValues.ContainsKey(argument))
                {
                    return cachedValues[argument];
                }

                TResult result = mathFunction(argument);
                cachedValues[argument] = result;

                return result;
            }
        }
    }
}

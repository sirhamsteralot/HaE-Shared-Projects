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
        public static class MonospaceUtils
	    {
            public static char GetColorChar(byte R, byte G, byte B)
            {
                GetSeven(ref R);
                GetSeven(ref G);
                GetSeven(ref B);

                return CreateCustomColor(R, G, B);
            }

            public static char GetColorChar(Color color)
            {
                return GetColorChar(color.R, color.G, color.B);
            }

            private static char CreateCustomColor(byte r, byte g, byte b)
            {
                return (char)(0xE100 + (MathHelper.Clamp(r, 0, 7) << 6) + (MathHelper.Clamp(g, 0, 7) << 3) + MathHelper.Clamp(b, 0, 7));
            }

            private static void GetSeven(ref byte B)
            {
                double I = B / 256.0;
                I *= 7;
                B = (byte)(int)Math.Round(I);
            }
        }
	}
}

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
        public partial class FontLibrary
        {
            public static class Numbers
            {
                public static bool[,] N0 = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { false, true, true, true, false },
                    { true, false, false, false, true },
                    { true, false, false, true, true },
                    { true, false, true, false, true },
                    { true, true, false, false, true },
                    { true, false, false, false, true },
                    { false, true, true, true, false }
                };

                public static bool[,] N1 = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { false, false, true, false, false },
                    { false, true, true, false, false },
                    { false, false, true, false, false },
                    { false, false, true, false, false },
                    { false, false, true, false, false },
                    { false, false, true, false, false },
                    { false, true, true, true, false }
                };

                public static bool[,] N2 = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { false, true, true, true, false },
                    { true, false, false, false, true },
                    { false, false, false, false, true },
                    { false, false, false, true, false },
                    { false, false, true, false, false },
                    { false, true, false, false, false },
                    { true, true, true, true, true }
                };

                public static bool[,] N3 = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, true, true, true, true },
                    { false, false, false, true, false },
                    { false, false, true, false, false },
                    { false, false, false, true, false },
                    { false, false, false, false, true },
                    { true, false, false, false, true },
                    { false, true, true, true, false }
                };

                public static bool[,] N4 = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { false, false, false, true, false },
                    { false, false, true, true, false },
                    { false, true, false, true, false },
                    { true, false, false, true, false },
                    { true, true, true, true, true },
                    { false, false, false, true, false },
                    { false, false, false, true, false }
                };

                public static bool[,] N5 = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, true, true, true, true },
                    { true, false, false, false, false },
                    { true, true, true, true, false },
                    { false, false, false, false, true },
                    { false, false, false, false, true },
                    { true, false, false, false, true },
                    { false, true, true, true, false }
                };

                public static bool[,] N6 = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { false, false, true, true, false },
                    { false, true, false, false, false },
                    { true, false, false, false, false },
                    { true, true, true, true, false },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { false, true, true, true, false }
                };

                public static bool[,] N7 = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, true, true, true, true },
                    { false, false, false, false, true },
                    { false, false, false, true, false },
                    { false, false, true, false, false },
                    { false, false, true, false, false },
                    { false, false, true, false, false },
                    { false, false, true, false, false }
                };

                public static bool[,] N8 = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { false, true, true, true, false },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { false, true, true, true, false },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { false, true, true, true, false }
                };

                public static bool[,] N9 = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { false, true, true, true, false },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { false, true, true, true, true },
                    { false, false, false, false, true },
                    { false, false, false, true, false },
                    { false, true, true, false, false }
                };

            }
        }
	}
}

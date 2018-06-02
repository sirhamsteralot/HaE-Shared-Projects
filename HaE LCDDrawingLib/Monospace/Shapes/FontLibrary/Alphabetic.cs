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
            public static class Alphabetic
            {
                public static bool[,] A = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { false, false, false, false, false },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, true, true, true, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true }
                };

                public static bool[,] B = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, true, true, true, false },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, true, true, true, false },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, true, true, true, false }
                };

                public static bool[,] C = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { false, true, true, true, false },
                    { true, false, false, false, true },
                    { true, false, false, false, false },
                    { true, false, false, false, false },
                    { true, false, false, false, false },
                    { true, false, false, false, true },
                    { false, true, true, true, false }
                };

                public static bool[,] D = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, true, true, false, false },
                    { true, false, false, true, false },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, true, false },
                    { true, true, true, false, false }
                };

                public static bool[,] E = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, true, true, true, true },
                    { true, false, false, false, false },
                    { true, false, false, false, false },
                    { true, true, true, true, false },
                    { true, false, false, false, false },
                    { true, false, false, false, false },
                    { true, true, true, true, true }
                };

                public static bool[,] F = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, true, true, true, true },
                    { true, false, false, false, false },
                    { true, false, false, false, false },
                    { true, true, true, true, false },
                    { true, false, false, false, false },
                    { true, false, false, false, false },
                    { true, false, false, false, false }
                };

                public static bool[,] G = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, true, true, true, false },
                    { true, false, false, false, true },
                    { true, false, false, false, false },
                    { true, false, true, true, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, true, true, true, true }
                };

                public static bool[,] H = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, true, true, true, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true }
                };

                public static bool[,] I = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, true, true, true, true },
                    { false, false, true, false, false },
                    { false, false, true, false, false },
                    { false, false, true, false, false },
                    { false, false, true, false, false },
                    { false, false, true, false, false },
                    { true, true, true, true, true }
                };

                public static bool[,] J = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { false, false, true, true, true },
                    { false, false, false, true, false },
                    { false, false, false, true, false },
                    { false, false, false, true, false },
                    { false, false, false, true, false },
                    { true, false, false, true, false },
                    { false, true, true, false, false }
                };

                public static bool[,] K = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, false, false, false, true },
                    { true, false, false, true, false },
                    { true, false, true, false, false },
                    { true, true, false, false, false },
                    { true, false, true, false, false },
                    { true, false, false, true, false },
                    { true, false, false, false, true }
                };

                public static bool[,] L = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, false, false, false, false },
                    { true, false, false, false, false },
                    { true, false, false, false, false },
                    { true, false, false, false, false },
                    { true, false, false, false, false },
                    { true, false, false, false, false },
                    { true, true, true, true, true }
                };

                public static bool[,] M = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, false, false, false, true },
                    { true, true, false, true, true },
                    { true, false, true, false, true },
                    { true, false, true, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true }
                };

                public static bool[,] N = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, true, false, false, true },
                    { true, false, true, false, true },
                    { true, false, false, true, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true }
                };

                public static bool[,] O = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { false, true, true, true, false },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { false, true, true, true, false }
                };

                public static bool[,] P = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, true, true, true, false },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, true, true, true, false },
                    { true, false, false, false, false },
                    { true, false, false, false, false },
                    { true, false, false, false, false }
                };

                public static bool[,] Q = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { false, true, true, true, false },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, true, false, true },
                    { true, false, false, true, false },
                    { false, true, true, false, true }
                };

                public static bool[,] R = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, true, true, true, false },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, true, true, true, false },
                    { true, false, true, false, false },
                    { true, false, false, true, false },
                    { true, false, false, false, true }
                };

                public static bool[,] S = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { false, true, true, true, true },
                    { true, false, false, false, false },
                    { true, false, false, false, false },
                    { false, true, true, true, false },
                    { false, false, false, false, true },
                    { false, false, false, false, true },
                    { true, true, true, true, false }
                };

                public static bool[,] T = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, true, true, true, true },
                    { false, false, true, false, false },
                    { false, false, true, false, false },
                    { false, false, true, false, false },
                    { false, false, true, false, false },
                    { false, false, true, false, false },
                    { false, false, true, false, false }
                };

                public static bool[,] U = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { false, true, true, true, false }
                };

                public static bool[,] V = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { false, true, false, true, false },
                    { false, false, true, false, false }
                };

                public static bool[,] W = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, true, false, true },
                    { true, false, true, false, true },
                    { true, false, true, false, true },
                    { false, true, false, true, false }
                };

                public static bool[,] X = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { false, true, false, true, false },
                    { false, false, true, false, false },
                    { false, true, false, true, false },
                    { true, false, false, false, true },
                    { true, false, false, false, true }
                };

                public static bool[,] Y = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { false, true, false, true, false },
                    { false, false, true, false, false },
                    { false, false, true, false, false },
                    { false, false, true, false, false }
                };

                public static bool[,] Z = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, true, true, true, true },
                    { false, false, false, false, true },
                    { false, false, false, true, false },
                    { false, false, true, false, false },
                    { false, true, false, false, false },
                    { true, false, false, false, false },
                    { true, true, true, true, true }
                };
            }
        }
	}
}

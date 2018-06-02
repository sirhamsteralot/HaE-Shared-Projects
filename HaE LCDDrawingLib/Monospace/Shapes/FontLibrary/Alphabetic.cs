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
            public class Alphabetic
            {
                public bool[,] A = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { false, false, false, false, false },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, true, true, true, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true }
                };

                public bool[,] B = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, true, true, true, false },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, true, true, true, false },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, true, true, true, false }
                };

                public bool[,] C = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { false, true, true, true, false },
                    { true, false, false, false, true },
                    { true, false, false, false, false },
                    { true, false, false, false, false },
                    { true, false, false, false, false },
                    { true, false, false, false, true },
                    { false, true, true, true, false }
                };

                public bool[,] D = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, true, true, false, false },
                    { true, false, false, true, false },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, true, false },
                    { true, true, true, false, false }
                };

                public bool[,] E = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, true, true, true, true },
                    { true, false, false, false, false },
                    { true, false, false, false, false },
                    { true, true, true, true, false },
                    { true, false, false, false, false },
                    { true, false, false, false, false },
                    { true, true, true, true, true }
                };

                public bool[,] F = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, true, true, true, true },
                    { true, false, false, false, false },
                    { true, false, false, false, false },
                    { true, true, true, true, false },
                    { true, false, false, false, false },
                    { true, false, false, false, false },
                    { true, false, false, false, false }
                };

                public bool[,] G = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, true, true, true, false },
                    { true, false, false, false, true },
                    { true, false, false, false, false },
                    { true, false, true, true, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, true, true, true, true }
                };

                public bool[,] H = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, true, true, true, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true }
                };

                public bool[,] I = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, true, true, true, true },
                    { false, false, true, false, false },
                    { false, false, true, false, false },
                    { false, false, true, false, false },
                    { false, false, true, false, false },
                    { false, false, true, false, false },
                    { true, true, true, true, true }
                };

                public bool[,] J = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { false, false, true, true, true },
                    { false, false, false, true, false },
                    { false, false, false, true, false },
                    { false, false, false, true, false },
                    { false, false, false, true, false },
                    { true, false, false, true, false },
                    { false, true, true, false, false }
                };

                public bool[,] K = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, false, false, false, true },
                    { true, false, false, true, false },
                    { true, false, true, false, false },
                    { true, true, false, false, false },
                    { true, false, true, false, false },
                    { true, false, false, true, false },
                    { true, false, false, false, true }
                };

                public bool[,] L = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, false, false, false, false },
                    { true, false, false, false, false },
                    { true, false, false, false, false },
                    { true, false, false, false, false },
                    { true, false, false, false, false },
                    { true, false, false, false, false },
                    { true, true, true, true, true }
                };

                public bool[,] M = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, false, false, false, true },
                    { true, true, false, true, true },
                    { true, false, true, false, true },
                    { true, false, true, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true }
                };

                public bool[,] N = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, true, false, false, true },
                    { true, false, true, false, true },
                    { true, false, false, true, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true }
                };

                public bool[,] O = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { false, true, true, true, false },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { false, true, true, true, false }
                };

                public bool[,] P = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, true, true, true, false },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, true, true, true, false },
                    { true, false, false, false, false },
                    { true, false, false, false, false },
                    { true, false, false, false, false }
                };

                public bool[,] Q = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { false, true, true, true, false },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, true, false, true },
                    { true, false, false, true, false },
                    { false, true, true, false, true }
                };

                public bool[,] R = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, true, true, true, false },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, true, true, true, false },
                    { true, false, true, false, false },
                    { true, false, false, true, false },
                    { true, false, false, false, true }
                };

                public bool[,] S = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { false, true, true, true, true },
                    { true, false, false, false, false },
                    { true, false, false, false, false },
                    { false, true, true, true, false },
                    { false, false, false, false, true },
                    { false, false, false, false, true },
                    { true, true, true, true, false }
                };

                public bool[,] T = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, true, true, true, true },
                    { false, false, true, false, false },
                    { false, false, true, false, false },
                    { false, false, true, false, false },
                    { false, false, true, false, false },
                    { false, false, true, false, false },
                    { false, false, true, false, false }
                };

                public bool[,] U = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { false, true, true, true, false }
                };

                public bool[,] V = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { false, true, false, true, false },
                    { false, false, true, false, false }
                };

                public bool[,] W = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, true, false, true },
                    { true, false, true, false, true },
                    { true, false, true, false, true },
                    { false, true, false, true, false }
                };

                public bool[,] X = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { false, true, false, true, false },
                    { false, false, true, false, false },
                    { false, true, false, true, false },
                    { true, false, false, false, true },
                    { true, false, false, false, true }
                };

                public bool[,] Y = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { false, true, false, true, false },
                    { false, false, true, false, false },
                    { false, false, true, false, false },
                    { false, false, true, false, false }
                };

                public bool[,] Z = new bool[CHARHEIGHT, CHARWIDTH]
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

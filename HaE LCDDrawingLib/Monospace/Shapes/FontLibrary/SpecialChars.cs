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
            public static class SpecialChars
            {
                public static bool[,] UnknownChar = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, true, true, true, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, true, true, true, true }
                };

                public static bool[,] Space = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { false, false, false, false, false },
                    { false, false, false, false, false },
                    { false, false, false, false, false },
                    { false, false, false, false, false },
                    { false, false, false, false, false },
                    { false, false, false, false, false },
                    { false, false, false, false, false }
                };

                public static bool[,] QuestionMark = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { false, true, true, false, false },
                    { false, false, false, true, false },
                    { false, false, false, true, false },
                    { false, false, false, true, false },
                    { false, false, true, false, false },
                    { false, false, false, false, false },
                    { false, false, true, false, false }
                };

                public static bool[,] ExclamationMark = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { false, false, true, false, false },
                    { false, false, true, false, false },
                    { false, false, true, false, false },
                    { false, false, true, false, false },
                    { false, false, true, false, false },
                    { false, false, false, false, false },
                    { false, false, true, false, false }
                };

                public static bool[,] Comma = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { false, false, false, false, false },
                    { false, false, false, false, false },
                    { false, false, false, false, false },
                    { false, false, false, false, false },
                    { false, false, true, true, false },
                    { false, false, false, true, false },
                    { false, false, true, false, false }
                };

                public static bool[,] Dot = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { false, false, false, false, false },
                    { false, false, false, false, false },
                    { false, false, false, false, false },
                    { false, false, false, false, false },
                    { false, false, false, false, false },
                    { false, false, true, true, false },
                    { false, false, true, true, false }
                };

                public static bool[,] DotComma = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { false, false, false, false, false },
                    { false, false, true, true, false },
                    { false, false, true, true, false },
                    { false, false, false, false, false },
                    { false, false, true, true, false },
                    { false, false, false, true, false },
                    { false, false, true, false, false }
                };

                public static bool[,] DoubleDot = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { false, false, false, false, false },
                    { false, false, true, true, false },
                    { false, false, true, true, false },
                    { false, false, false, false, false },
                    { false, false, true, true, false },
                    { false, false, true, true, false },
                    { false, false, false, false, false }
                };

                public static bool[,] UnderScore = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { false, false, false, false, false },
                    { false, false, false, false, false },
                    { false, false, false, false, false },
                    { false, false, false, false, false },
                    { false, false, false, false, false },
                    { false, false, false, false, false },
                    { true, true, true, true, true }
                };

                public static bool[,] Slash = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { false, false, false, false, false },
                    { false, false, false, false, true },
                    { false, false, false, true, false },
                    { false, false, true, false, false },
                    { false, true, false, false, false },
                    { true, false, false, false, false },
                    { false, false, false, false, false }
                };

                public static bool[,] BackSlash = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { false, false, false, false, false },
                    { true, false, false, false, false },
                    { false, true, false, false, false },
                    { false, false, true, false, false },
                    { false, false, false, true, false },
                    { false, false, false, false, true },
                    { false, false, false, false, false }
                };

                public static bool[,] OpeningParenthesis = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { false, false, true, false, false },
                    { false, true, false, false, false },
                    { true, false, false, false, false },
                    { true, false, false, false, false },
                    { true, false, false, false, false },
                    { false, true, false, false, false },
                    { false, false, true, false, false }
                };

                public static bool[,] ClosingParenthesis = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { false, false, true, false, false },
                    { false, false, false, true, false },
                    { false, false, false, false, true },
                    { false, false, false, false, true },
                    { false, false, false, false, true },
                    { false, false, false, true, false },
                    { false, false, true, false, false }
                };

                public static bool[,] Pipe = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { false, false, true, false, false },
                    { false, false, true, false, false },
                    { false, false, true, false, false },
                    { false, false, true, false, false },
                    { false, false, true, false, false },
                    { false, false, true, false, false },
                    { false, false, true, false, false }
                };

                public static bool[,] Percent = new bool[CHARHEIGHT, CHARWIDTH]
                {
                    { true, true, false, false, false },
                    { true, true, false, false, true },
                    { false, false, false, true, false },
                    { false, false, true, false, false },
                    { false, true, false, false, false },
                    { true, false, false, true, true },
                    { false, false, false, true, true }
                };
            }
        }
	}
}

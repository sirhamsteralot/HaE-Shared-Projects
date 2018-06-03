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
        public static partial class FontLibrary
	    {
            public const int CHARWIDTH = 5;
            public const int CHARHEIGHT = 7;

            public static bool[,] GetCharacterBitmap(char character)
            {
                switch (character)
                {
                    #region Alphabetic
                    case 'A':
                        return Alphabetic.A;
                    case 'B':
                        return Alphabetic.B;
                    case 'C':
                        return Alphabetic.C;
                    case 'D':
                        return Alphabetic.D;
                    case 'E':
                        return Alphabetic.E;
                    case 'F':
                        return Alphabetic.F;
                    case 'G':
                        return Alphabetic.G;
                    case 'H':
                        return Alphabetic.H;
                    case 'I':
                        return Alphabetic.I;
                    case 'J':
                        return Alphabetic.J;
                    case 'K':
                        return Alphabetic.K;
                    case 'L':
                        return Alphabetic.L;
                    case 'M':
                        return Alphabetic.M;
                    case 'N':
                        return Alphabetic.N;
                    case 'O':
                        return Alphabetic.O;
                    case 'P':
                        return Alphabetic.P;
                    case 'Q':
                        return Alphabetic.Q;
                    case 'R':
                        return Alphabetic.R;
                    case 'S':
                        return Alphabetic.S;
                    case 'T':
                        return Alphabetic.T;
                    case 'U':
                        return Alphabetic.U;
                    case 'V':
                        return Alphabetic.V;
                    case 'W':
                        return Alphabetic.W;
                    case 'X':
                        return Alphabetic.X;
                    case 'Y':
                        return Alphabetic.Y;
                    case 'Z':
                        return Alphabetic.Z;
                    #endregion
                    #region SpecialChars
                    case ' ':
                        return SpecialChars.Space;
                    #endregion
                    default:
                        return SpecialChars.UnknownChar;
                }
            }
	    }
	}
}

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
        public static class DebugUtils
	    {
	        public static void MainWrapper(Action<string, UpdateType> Main, string Argument, UpdateType utype, Program P)
            {
                try
                {
                    Main(Argument, utype);
                } catch (Exception e)
                {
                    var sb = new StringBuilder();

                    sb.AppendLine("Exception Message:");
                    sb.AppendLine($"   {e.Message}");
                    sb.AppendLine();

                    sb.AppendLine("Stack trace:");
                    sb.AppendLine(e.StackTrace);
                    sb.AppendLine();

                    var exceptionDump = sb.ToString();

                    P.Echo(exceptionDump);
                    throw;
                }
            }
	    }
	}
}

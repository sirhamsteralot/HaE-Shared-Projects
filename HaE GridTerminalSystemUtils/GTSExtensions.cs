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
    // This template is intended for extension classes. For most purposes you're going to want a normal
    // utility class.
    // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods
    static class GTSExtensions
	{
        /*============|   IMyCubeGrid  |============*/
        public static void GetCubesOfType<T>(this IMyCubeGrid cubegrid, List<T> cubeList) where T : class
        {
            for (int x = cubegrid.Min.X; x <= cubegrid.Max.X; x++)
            {
                for (int y = cubegrid.Min.Y; y <= cubegrid.Max.Y; y++)
                {
                    for (int z = cubegrid.Min.Z; z <= cubegrid.Max.Z; z++)
                    {
                        var vec = new Vector3I(x, y, z);
                        var slimblock = cubegrid.GetCubeBlock(vec);
                        T stator = null;

                        if (slimblock != null)
                            stator = slimblock.FatBlock as T;

                        if (stator != null)
                            cubeList.Add(stator);
                    }
                }
            }
        }

        public static void GetCubesOfType<T>(this IMyCubeGrid cubegrid, IMyGridTerminalSystem GTS, List<T> cubeList) where T : class, IMyTerminalBlock
        {
            GTS.GetBlocksOfType(cubeList, x => x.CubeGrid.EntityId == cubegrid.EntityId);
        }

        /*============| ShipController |============*/
        public static Vector3D GetVelocityVector(this IMyShipController controller)
        {
            return controller.GetShipVelocities().LinearVelocity;
        }

        /*=============| IMyCubeBlock |=============*/
        public static bool IsClosed(this IMyCubeBlock block)
        {
            return block.WorldMatrix == MatrixD.Identity;
        }
    }
}

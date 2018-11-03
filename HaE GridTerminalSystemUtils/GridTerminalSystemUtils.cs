﻿using Sandbox.Game.EntityComponents;
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
        public class GridTerminalSystemUtils
	    {
            public IMyProgrammableBlock Me;
            public IMyGridTerminalSystem GridTerminalSystem;

            public GridTerminalSystemUtils(IMyProgrammableBlock me, IMyGridTerminalSystem gridTerminalSystem)
            {
                Me = me;
                GridTerminalSystem = gridTerminalSystem;
            }

            public IMyTerminalBlock GetBlockWithNameOnGrid(string name)
            {
                List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
                GridTerminalSystem.SearchBlocksOfName(name, blocks, x => x.CubeGrid == Me.CubeGrid);

                if (blocks.Count < 1)
                    return null;

                return (blocks[0]);
            }

            public void GetBlocksOfTypeOnGrid<T>(List<T> blocks) where T : class, IMyTerminalBlock
            {
                GridTerminalSystem.GetBlocksOfType(blocks, x => x.CubeGrid == Me.CubeGrid);
            }
        }
	}
}

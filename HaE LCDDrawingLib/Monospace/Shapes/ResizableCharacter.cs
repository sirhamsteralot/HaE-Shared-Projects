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
        public class ResizableCharacter : IMonoElement
        {
            public Vector2I Position { get { return position; } set { position = value; } }
            public Color Color { get { return color; } set { color = value; Generate(); } }
            public Canvas Canvas { get { return canvas; } set { canvas = value; } }

            private Vector2I position;
            private Color color;
            private Canvas canvas;

            private float size;
            private char character;

            public ResizableCharacter(Vector2I position, char character, Color color, float size)
            {
                this.position = position;

                int sizeX = (int)(FontLibrary.CHARWIDTH * size) + 1;
                int sizeY = (int)(FontLibrary.CHARHEIGHT * size) + 1;

                this.color = color;
                this.size = size;

                canvas = new Canvas(sizeX, sizeY);
                Generate();
            }

            private void Generate()
            {
                char pixel = MonospaceUtils.GetColorChar(color);
                canvas.Clear();


            }

            public Canvas Draw()
            {
                return canvas;
            }
        }
    }
}

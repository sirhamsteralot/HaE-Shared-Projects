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

            private int size;
            private bool[,] characterbitMap;

            private Vector2I center;

            public ResizableCharacter(Vector2I position, char character, Color color, int size)
            {
                this.position = position;

                int sizeX = (FontLibrary.CHARWIDTH * size) + 2;
                int sizeY = (FontLibrary.CHARHEIGHT * size) + 2;

                center = new Vector2I((FontLibrary.CHARWIDTH * size) / 2 + 1, (FontLibrary.CHARHEIGHT * size) / 2);

                this.color = color;
                this.size = size;

                canvas = new Canvas(sizeX, sizeY);

                characterbitMap = FontLibrary.GetCharacterBitmap(character);

                Generate();
            }

            private void Generate()
            {
                char pixel = MonospaceUtils.GetColorChar(color);
                canvas.Clear();

                canvas.PaintBitMap(characterbitMap, color, center.X, center.Y);
            }

            public Canvas Draw()
            {
                return canvas;
            }
        }
    }
}

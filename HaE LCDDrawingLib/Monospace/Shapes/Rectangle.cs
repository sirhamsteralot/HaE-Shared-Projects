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
        public class Rectangle : IMonoElement
        {
            public int Radius { get { return radius; } set { radius = value; } }
            public Vector2I Position { get { return position; } set { position = value; } }
            public bool Fill { get { return fill; } set { fill = value; } }
            public Color Color { get { return color; } set { color = value; } }
            public Canvas Canvas { get { return canvas; } set { canvas = value; } }

            private Vector2I position;
            private int radius;
            private bool fill;
            private Color color;
            private Canvas canvas;

            public Rectangle(Vector2I position, int radius, bool fill)
            {
                this.position = position;
                this.radius = radius;

                int sizeX = radius * 2;
                int sizeY = radius * 2;

                canvas = new Canvas(sizeX, sizeY);
            }

            private void Generate()
            {
                char pixel = MonospaceUtils.GetColorChar(color);

                for (int x = 0; x < canvas.sizeX; x++)
                {
                    for (int y = 0; y < canvas.sizeY; y++)
                    {
                        if (fill)
                        {
                            if (((x - radius) * (x - radius) + (y - radius) * (y - radius)) <= radius * radius)
                            {
                                canvas.PaintPixel(pixel, x, y);
                            }
                        }
                        else
                        {
                            if (Math.Abs(((x - radius) * (x - radius) + (y - radius) * (y - radius)) - (radius * radius)) <= 1)
                            {
                                canvas.PaintPixel(pixel, x, y);
                            }
                        }
                    }
                }
            }

            public Canvas Draw()
            {
                return canvas;
            }
        }
    }
}

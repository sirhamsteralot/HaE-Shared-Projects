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
        public class Circle : IMonoElement
        {
            public int Radius { get { return radius; } set { radius = value; Generate(); } }
            public Vector2I Position { get { return position; } set { position = value; } }
            public bool Fill { get { return fill; } set { fill = value; Generate(); } }
            public Color Color { get { return color; } set { color = value; Generate(); } }
            public Canvas Canvas { get { return canvas; } set { canvas = value; } }

            private Vector2I position;
            private int radius;
            private bool fill;
            private Color color;
            private Canvas canvas;

            private Vector2I center;

            public Circle(Vector2I position, int radius, Color color, bool fill)
            {
                this.position = position;
                this.radius = radius;
                this.fill = fill;

                int sizeX = radius * 2;
                int sizeY = radius * 2;

                center = new Vector2I(radius, radius);
                this.color = color;

                canvas = new Canvas(sizeX, sizeY);
                Generate();
            }

            private void Generate()
            {
                char pixel = MonospaceUtils.GetColorChar(color);
                canvas.Clear();

                for (int x = 0; x < canvas.sizeX; x++)
                {
                    for (int y = 0; y < canvas.sizeY; y++)
                    {
                        if (fill)
                        {
                            var thing = ((x - center.X) * (x - center.X) + (y - center.Y) * (y - center.Y));
                            if (thing <= radius * radius)
                            {
                                canvas.PaintPixel(pixel, x, y);
                            }
                        } else
                        {
                            if (Math.Abs(((x - center.X) * (x - center.X) + (y - center.Y) * (y - center.Y)) - (radius * radius)) <= 1)
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

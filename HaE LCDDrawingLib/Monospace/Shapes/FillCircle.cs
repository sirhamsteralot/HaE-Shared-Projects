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
        public class FillCircle : IComplexElement
	    {
            public int Radius { get { return radius; } set { radius = value; Generate(); } }
            public Vector2I Position { get { return position; } set { position = value; } }
            public Color Color { get { return color; } set { color = value; Generate(); } }
            public Canvas Canvas { get { return canvas; } set { canvas = value; } }

            private Vector2I position;
            private int radius;
            private Color color;
            private Canvas canvas;

            private int lineThickness;
            private Vector2I center;

            public FillCircle(Vector2I position, int radius, Color color, int lineThickness = 4)
            {
                canvas.Clear();

                this.position = position;
                this.radius = radius;

                int sizeX = radius * 2 + 1;
                int sizeY = radius * 2 + 1;

                center = new Vector2I(radius, radius);
                this.color = color;
                this.lineThickness = lineThickness;

                canvas = new Canvas(sizeX, sizeY);
                Generate();
            }

            public IEnumerator<bool> Generate()
            {
                char pixel = MonospaceUtils.GetColorChar(color);
                canvas.Clear();

                int x = radius - 1;
                int y = 0;
                int dx = 1;
                int dy = 1;
                int err = dx - (radius << 1);

                while (x >= y)
                {
                    MonospaceUtils.PlotLine(center.X - x, center.Y + y, center.X + x, center.Y + y, canvas, pixel);
                    MonospaceUtils.PlotLine(center.X - y, center.Y + x, center.X + y, center.Y + x, canvas, pixel);
                    MonospaceUtils.PlotLine(center.X - x, center.Y - y, center.X + x, center.Y - y, canvas, pixel);
                    MonospaceUtils.PlotLine(center.X - y, center.Y - x, center.X + y, center.Y - x, canvas, pixel);


                    if (err <= 0)
                    {
                        y++;
                        err += dy;
                        dy += 2;
                    }

                    if (err > 0)
                    {
                        x--;
                        dx += 2;
                        err += dx - (radius << 1);
                    }

                    yield return true;
                }
            }

            public Canvas Draw()
            {
                return canvas;
            }
        }
	}
}

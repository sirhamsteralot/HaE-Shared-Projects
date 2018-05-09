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
        public class Line : IMonoElement
        {
            public Vector2I Position { get { return position; } set { position = value;}}
            private Vector2I position;

            private Canvas canvas;
            private Color color;
            private Vector2I startPos;
            private Vector2I endPos;

            private Vector2I localStart;
            private Vector2I size;

            public Line(Vector2I startPos, Vector2I endPos, Color color)
            {
                this.color = color;
                this.startPos = startPos;
                this.endPos = endPos;

                size = endPos - startPos;

                position = startPos + size / 2;

                if (size.X < 0)
                    localStart.X = -size.X;

                if (size.Y < 0)
                    localStart.Y = -size.Y;

                int sizeX = Math.Abs(size.X);
                int sizeY = Math.Abs(size.Y);

                canvas = new Canvas(sizeX + 1, sizeY + 1);
                Generate();
            }

            public void Generate()
            {
                canvas.Clear();

                int x0 = localStart.X;
                int y0 = localStart.Y;

                int x1 = localStart.X + size.X;
                int y1 = localStart.Y + size.Y;

                char pixel = MonospaceUtils.GetColorChar(color);

                int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
                int dy = -Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
                int err = dx + dy, e2; /* error value e_xy */

                

                while(true)
                {  /* loop */
                    canvas.PaintPixel(pixel, x0, y0);
                    if (x0 == x1 && y0 == y1) break;
                    e2 = 2 * err;
                    if (e2 >= dy) { err += dy; x0 += sx; } /* e_xy+e_x > 0 */
                    if (e2 <= dx) { err += dx; y0 += sy; } /* e_xy+e_y < 0 */
                }
            }

            public Canvas Draw()
            {
                return canvas;
            }
        }
    }
}

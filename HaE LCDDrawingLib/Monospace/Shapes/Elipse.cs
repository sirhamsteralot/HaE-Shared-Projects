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
        public class Elipse : IMonoElement
        {
            public Vector2I Position { get { return position; } set { position = value; } }
            private Vector2I position;

            private Canvas canvas;
            private Color color;
            private Vector2I startPos;
            private Vector2I endPos;

            private Vector2I localStart;
            private Vector2I size;

            public Elipse(Vector2I startPos, Vector2I endPos, Color color)
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

                canvas = new Canvas(sizeX, sizeY);

                Generate();
            }

            public void Generate()
            {
                canvas.Clear();

                int x0 = localStart.X;
                int y0 = localStart.Y;

                int x1 = localStart.X + size.X;
                int y1 = localStart.Y + size.Y;

                int a = Math.Abs(x1 - x0), b = Math.Abs(y1 - y0), b1 = b & 1; /* values of diameter */
                long dx = 4 * (1 - a) * b * b, dy = 4 * (b1 + 1) * a * a; /* error increment */
                long err = dx + dy + b1 * a * a, e2; /* error of 1.step */

                if (x0 > x1) { x0 = x1; x1 += a; } /* if called with swapped points */
                if (y0 > y1) y0 = y1; /* .. exchange them */
                y0 += (b + 1) / 2; y1 = y0 - b1;   /* starting pixel */
                a *= 8 * a; b1 = 8 * b * b;

                char pixel = MonospaceUtils.GetColorChar(color);

                do
                {
                    canvas.PaintPixel(pixel, x1, y0); /*   I. Quadrant */
                    canvas.PaintPixel(pixel, x0, y0); /*  II. Quadrant */
                    canvas.PaintPixel(pixel, x0, y1); /* III. Quadrant */
                    canvas.PaintPixel(pixel, x1, y1); /*  IV. Quadrant */

                    e2 = 2 * err;
                    if (e2 <= dy) { y0++; y1--; err += dy += a; }  /* y step */
                    if (e2 >= dx || 2 * err > dy) { x0++; x1--; err += dx += b1; } /* x step */
                } while (x0 <= x1);

                while (y0 - y1 < b)
                {  /* too early stop of flat ellipses a=1 */
                    canvas.PaintPixel(pixel, x0 - 1, y0); /* -> finish tip of ellipse */
                    canvas.PaintPixel(pixel, x1 + 1, y0++);
                    canvas.PaintPixel(pixel, 0 - 1, y1);
                    canvas.PaintPixel(pixel, x1 + 1, y1--);
                }
            }

            public Canvas Draw()
            {
                return canvas;
            }
        }
    }
}

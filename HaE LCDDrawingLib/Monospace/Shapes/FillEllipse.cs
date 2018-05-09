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
        public class FillEllipse : IComplexElement
	    {
            public Vector2I Position { get { return position; } set { position = value; } }
            private Vector2I position;

            private Canvas canvas;
            private Color color;
            private Vector2I startPos;
            private Vector2I endPos;

            private Vector2I localStart;
            private Vector2I size;

            int sizeX;
            int sizeY;

            public FillEllipse(Vector2I startPos, Vector2I endPos, Color color)
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

                sizeX = Math.Abs(size.X);
                sizeY = Math.Abs(size.Y);

                canvas = new Canvas(sizeX + 1, sizeY + 1);
            }

            /*====| Code From: http://enchantia.com/graphapp/doc/tech/ellipses.html |====*/
            public IEnumerator<bool> Generate()
            {
                canvas.Clear();
                int divX = sizeX / 2;
                int divY = sizeY / 2;

                int xc = divX;
                int yc = divY;
                int a = divX;
                int b = divY;


                int x = 0, y = b;
                int rx = x, ry = y;
                int width = 1;
                int height = 1;
                long a2 = (long)a * a, b2 = (long)b * b;
                long crit1 = -(a2 / 4 + a % 2 + b2);
                long crit2 = -(b2 / 4 + b % 2 + a2);
                long crit3 = -(b2 / 4 + b % 2);
                long t = -a2 * y; /* e(x+1/2,y-1/2) - (a^2+b^2)/4 */
                long dxt = 2 * b2 * x, dyt = -2 * a2 * y;
                long d2xt = 2 * b2, d2yt = 2 * a2;

                if (b == 0)
                {
                    FillRect(xc - a, yc, 2 * a + 1, 1);
                    yield return false;
                }

                while (y >= 0 && x <= a)
                {
                    if (t + b2 * x <= crit1 ||     /* e(x+1,y-1/2) <= 0 */
                        t + a2 * y <= crit3)
                    {     /* e(x+1/2,y) <= 0 */
                        if (height == 1)
                            ; /* draw nothing */
                        else if (ry * 2 + 1 > (height - 1) * 2)
                        {
                            FillRect(xc - rx, yc - ry, width, height - 1);
                            FillRect(xc - rx, yc + ry + 1, width, 1 - height);
                            ry -= height - 1;
                            height = 1;
                        }
                        else
                        {
                            FillRect(xc - rx, yc - ry, width, ry * 2 + 1);
                            ry -= ry;
                            height = 1;
                        }

                        x++; dxt += d2xt; t += dxt;

                        rx++;
                        width += 2;
                    }
                    else if (t - a2 * y > crit2)
                    { /* e(x+1/2,y-1) > 0 */
                        y--; dyt += d2yt; t += dyt;
                        height++;
                    }
                    else
                    {
                        if (ry * 2 + 1 > height * 2)
                        {
                            var task = new Task(FillRect(xc - rx, yc - ry, width, height));
                            while (task.MoveNext())
                                yield return true;

                            task = new Task(FillRect(xc - rx, yc + ry + 1, width, -height));
                            while (task.MoveNext())
                                yield return true;
                        }
                        else
                        {
                            var task = new Task(FillRect(xc - rx, yc - ry, width, ry * 2 + 1));
                            while (task.MoveNext())
                                yield return true;
                        }

                        x++; dxt += d2xt; t += dxt;

                        y--; dyt += d2yt; t += dyt;
                        rx++;
                        width += 2;
                        ry -= height;
                        height = 1;
                    }

                    yield return true;
                }

                if (ry > height)
                {
                    var task = new Task(FillRect(xc - rx, yc - ry, width, height));
                    while (task.MoveNext())
                        yield return true;

                    task = new Task(FillRect(xc - rx, yc + ry + 1, width, -height));
                    while (task.MoveNext())
                        yield return true;
                }
                else
                {
                    var task = new Task(FillRect(xc - rx, yc - ry, width, ry * 2 + 1));
                    while (task.MoveNext())
                        yield return true;
                }
            }

            private IEnumerator<bool> FillRect(int x, int y, int width, int height)
            {
                Vector2I startPos = new Vector2I(x,y);
                Vector2I endPos = startPos + new Vector2I(width,height);

                var rectangle = new FillRectangle(startPos, endPos, color);

                var task = new Task(rectangle.Generate());

                while (task.MoveNext())
                    yield return true;

                task = new Task(canvas.MergeCanvas(rectangle.Draw(), rectangle.Position, 100));

                while (task.MoveNext())
                    yield return true;
            }

            public Canvas Draw()
            {
                return canvas;
            }
        }
	}
}

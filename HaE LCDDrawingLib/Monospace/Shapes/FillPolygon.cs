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
        public class FillPolygon : IComplexElement
	    {
            public Vector2I Position { get { return position; } set { position = value; } }
            private Vector2I position;

            private Vector2[] positions;
            private List<Line> lines = new List<Line>();

            private Canvas canvas;
            private Color color;

            private Vector2I size;

            Vector2I topLeft;
            private int leftmostX = int.MaxValue;
            private int topmostY = int.MaxValue;

            Vector2I bottomRight;
            private int rightMostX = 0;
            private int bottomMostY = 0;

            public FillPolygon(List<Vector2I> positions, Color color)
            {
                this.color = color;
                this.positions = new Vector2[positions.Count];
                
                for (int i = 0; i < positions.Count; i++)
                {
                    this.positions[i] = positions[i];
                }

                foreach (var pos in this.positions)
                {
                    if (pos.X < leftmostX)
                        leftmostX = (int)pos.X;

                    if (pos.Y < topmostY)
                        topmostY = (int)pos.Y;

                    if (pos.X > rightMostX)
                        rightMostX = (int)pos.X;

                    if (pos.Y > bottomMostY)
                        bottomMostY = (int)pos.Y;
                }
                topLeft = new Vector2I(leftmostX, topmostY);
                bottomRight = new Vector2I(rightMostX, bottomMostY);

                size = bottomRight - topLeft;


                position = topLeft + size / 2;

                canvas = new Canvas(Math.Abs(size.X) + 10, Math.Abs(size.Y) + 10);

                size.X = Math.Abs(size.X);
                size.Y = Math.Abs(size.Y);
            }

            public IEnumerator<bool> Generate()
            {
                char pixel = MonospaceUtils.GetColorChar(color);
                int truecount = 0;

                for (int y = 0; y < size.Y; y++)
                {
                    for (int x = 0; x < size.X; x++)
                    {
                        if (PointInPolygon(x, y))
                        {
                            canvas.PaintPixel(pixel, x, y);
                            truecount++;
                        }
                    }
                    yield return true;
                }
            }


            bool PointInPolygon(float x, float y)
            {

                int i, j = positions.Length - 1;
                bool oddNodes = false;

                for (i = 0; i < positions.Length; i++)
                {
                    var posI = positions[i] - topLeft;
                    var posJ = positions[j] - topLeft;

                    if (posI.Y < y && positions[j].Y >= y
                    || posJ.Y < y && posI.Y >= y)
                    {
                        if (posI.X + (y - posI.Y) / (posJ.Y - posI.Y) * (posJ.X - posI.X) < x)
                        {
                            oddNodes = !oddNodes;
                        }
                    }
                    j = i;
                }

                return oddNodes;
            }

            public Canvas Draw()
            {
                return canvas;
            }
        }
	}
}

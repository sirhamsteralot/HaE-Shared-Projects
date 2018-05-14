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

            private List<Vector2I> positions;

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
                this.positions = positions;

                foreach (var pos in positions)
                {
                    if (pos.X < leftmostX)
                        leftmostX = pos.X;

                    if (pos.Y < topmostY)
                        topmostY = pos.Y;

                    if (pos.X > rightMostX)
                        rightMostX = pos.X;

                    if (pos.Y > bottomMostY)
                        bottomMostY = pos.Y;
                }
                topLeft = new Vector2I(leftmostX, topmostY);
                bottomRight = new Vector2I(rightMostX, bottomMostY);

                size = bottomRight - topLeft;


                position = topLeft + size / 2;

                canvas = new Canvas(Math.Abs(size.X) + 10, Math.Abs(size.Y) + 10);
            }

            public IEnumerator<bool> Generate()
            {
                char pixel = MonospaceUtils.GetColorChar(color);

                for (int i = 0; i < positions.Count - 1; i++)
                {
                    yield return true;

                    var currentPos = positions[i] - topLeft;
                    var nextpos = positions[i + 1] - topLeft;

                    Line line = new Line(currentPos, nextpos, color);
                    canvas.MergeCanvas(line.Draw(), line.Position);
                }

                yield return true;

                var lastPos = positions[positions.Count - 1] - topLeft;
                var firstPos = positions[0] - topLeft;

                Line finalLine = new Line(lastPos, firstPos, color);
                canvas.MergeCanvas(finalLine.Draw(), finalLine.Position);
            }

            public Canvas Draw()
            {
                return canvas;
            }


            //  Globals which should be set before calling this function:
            //
            //  int    polyCorners  =  how many corners the polygon has (no repeats)
            //  float  polyX[]      =  horizontal coordinates of corners
            //  float  polyY[]      =  vertical coordinates of corners
            //  float  x, y         =  point to be tested
            //
            //  (Globals are used in this example for purposes of speed.  Change as
            //  desired.)
            //
            //  The function will return YES if the point x,y is inside the polygon, or
            //  NO if it is not.  If the point is exactly on the edge of the polygon,
            //  then the function may return YES or NO.
            //
            //  Note that division by zero is avoided because the division is protected
            //  by the "if" clause which surrounds it.

            private bool PointInPolygon(int x, int y)
            {

                int i, j = positions.Count - 1;
                bool oddNodes = false;

                for (i = 0; i < positions.Count; i++)
                {
                    if (positions[i].Y < y && positions[j].Y >= y
                    || positions[j].Y < y && positions[i].Y >= y)
                    {
                        if (positions[i].X + (y - positions[i].Y) / (positions[j].Y - positions[i].Y) * (positions[j].X - positions[i].X) < x)
                        {
                            oddNodes = !oddNodes;
                        }
                    }
                    j = i;
                }

                return oddNodes;
            }
        }
	}
}

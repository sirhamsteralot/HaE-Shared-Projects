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
        public class Polygon : IComplexElement
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

            public Polygon(List<Vector2I> positions, Color color)
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
                

                position = topLeft + size/2;

                canvas = new Canvas(Math.Abs(size.X) + 10, Math.Abs(size.Y) + 10);
            }

            public IEnumerator<bool> Generate()
            {
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
        }
	}
}

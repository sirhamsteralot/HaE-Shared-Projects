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
            public Vector2I Position { get { return position; } set { position = value; } }
            private Vector2I position;

            private Canvas canvas;
            private Color color;
            private Vector2I startPos;
            private Vector2I endPos;

            private Vector2I size;

            private int sizeX;
            private int sizeY;

            public Rectangle(Vector2I startPos, Vector2I endPos, Color color)
            {
                this.color = color;
                this.startPos = startPos;
                this.endPos = endPos;

                size = endPos - startPos;

                position = startPos + size / 2;

                sizeX = Math.Abs(size.X);
                sizeY = Math.Abs(size.Y);

                canvas = new Canvas(sizeX + 1, sizeY + 1);

                Generate();
            }

            public void Generate()
            {
                canvas.Clear();

                P.Echo("Generating...");

                Vector2I topLeft = Vector2I.Zero;
                Vector2I topRight = new Vector2I(sizeX, 0);
                Vector2I bottomLeft = new Vector2I(0, sizeY);
                Vector2I bottomRight = new Vector2I(sizeX,sizeY);

                Line top = new Line(topLeft, topRight, color);
                Line left = new Line(topLeft, bottomLeft, color);
                Line right = new Line(topRight, bottomRight, color);
                Line bottom = new Line(bottomLeft, bottomRight, color);

                P.Echo("Generated lines...");
                P.Echo($"{topLeft}; {bottomRight}");

                canvas.MergeCanvas(top.Draw(), top.Position);
                canvas.MergeCanvas(left.Draw(), left.Position);
                canvas.MergeCanvas(right.Draw(), right.Position);
                canvas.MergeCanvas(bottom.Draw(), bottom.Position);

                P.Echo("Generated.");
            }

            public Canvas Draw()
            {
                return canvas;
            }
        }
    }
}

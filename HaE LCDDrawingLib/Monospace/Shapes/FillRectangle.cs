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
        public class FillRectangle : IComplexElement 
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

            public FillRectangle(Vector2I startPos, Vector2I endPos, Color color)
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

            public IEnumerator<bool> Generate()
            {
                canvas.Clear();

                return canvas.SetBackGround(color, 100);
            }

            public Canvas Draw()
            {
                return canvas;
            }
        }
	}
}

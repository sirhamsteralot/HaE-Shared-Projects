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
        public class MonospaceDrawingLib
	    {
            private Canvas mainCanvas;
            private HashSet<IMonoElement> elements = new HashSet<IMonoElement>();

            public Color backgroundColor;

            public MonospaceDrawingLib(int sizeX, int sizeY, Color background)
            {
                backgroundColor = background;

                mainCanvas = new Canvas(sizeX, sizeY);
            }

            public void AddElement(IMonoElement element)
            {
                elements.Add(element);
            }

            public IEnumerator<bool> Generate()
            {
                mainCanvas.SetBackGround(backgroundColor);
                var tempElements = new HashSet<IMonoElement>(elements);

                foreach (var element in tempElements)
                {
                    yield return true;
                    mainCanvas.MergeCanvas(element.Draw(), element.Position);
                }

                for (int y = 0; y < mainCanvas.sizeY; y++)
                {
                    yield return true;
                    mainCanvas.PaintPixel('\n', mainCanvas.sizeX, y);
                }
            }

            public StringBuilder Draw()
            {
                return mainCanvas.ToStringBuilder();
            }
        }
	}
}

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

            private Scheduler internalRenderScheduler = new Scheduler();

            public Color backgroundColor;

            public MonospaceDrawingLib(int sizeX, int sizeY, Color background)
            {
                backgroundColor = background;

                mainCanvas = new Canvas(sizeX, sizeY);
            }

            public void AddElement(IMonoElement element)
            {
                elements.Add(element);
                internalRenderScheduler.AddTask(RenderAddedElement(element));
            }

            public void SetBackground(Color color)
            {
                mainCanvas.SetBackGround(color);
            }

            public IEnumerator<bool> ReGenerate()
            {
                Task renderBackgroundTask = new Task(mainCanvas.SetBackGround(backgroundColor, 100));
                while (renderBackgroundTask.MoveNext())
                    yield return true;

                var tempElements = new HashSet<IMonoElement>(elements);

                foreach (var element in tempElements)
                {
                    yield return true;

                    Task internalRenderTask = new Task(mainCanvas.MergeCanvas(element.Draw(), element.Position, 100));
                    while (internalRenderTask.MoveNext())
                        yield return true;
                }
            }

            public IEnumerator<bool> RenderAddedElement(IMonoElement element)
            {
                Task internalRenderTask = new Task(mainCanvas.MergeCanvas(element.Draw(), element.Position, 100));
                while (internalRenderTask.MoveNext())
                    yield return true;
            }

            public StringBuilder Draw()
            {
                internalRenderScheduler.Main();

                return mainCanvas.ToStringBuilder();
            }
        }
	}
}

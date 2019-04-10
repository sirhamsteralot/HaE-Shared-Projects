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

            public int RenderQueueSize => internalRenderScheduler.QueueCount;
            public Action onRenderDone;

            private Canvas mainCanvas;
            private HashSet<IMonoElement> elements = new HashSet<IMonoElement>();

            private HashSet<IRuntimeUpdatable> runtimeUpdatableElements = new HashSet<IRuntimeUpdatable>();

            private Scheduler internalRenderScheduler = new Scheduler();
            private Color backgroundColor;

            public MonospaceDrawingLib(int sizeX, int sizeY, Color background)
            {
                backgroundColor = background;

                mainCanvas = new Canvas(sizeX, sizeY);
            }

            public void AddRenderTask(IEnumerator<bool> task, Action callback)
            {
                internalRenderScheduler.AddTask(task, callback);
            }

            public void AddElement(IMonoElement element)
            {
                elements.Add(element);
                internalRenderScheduler.AddTask(RenderAddedElement(element), onRenderDone);

                AddRuntimeUpdatable(element as IRuntimeUpdatable);
            }

            public void AddElement(IComplexElement element)
            {
                internalRenderScheduler.AddTask(element.Generate());

                elements.Add(element);
                internalRenderScheduler.AddTask(RenderAddedElement(element), onRenderDone);

                AddRuntimeUpdatable(element as IRuntimeUpdatable);
            }

            private void AddRuntimeUpdatable(IRuntimeUpdatable runtimeUpdatable)
            {
                if (runtimeUpdatable == null)
                    return;

                runtimeUpdatableElements.Add(runtimeUpdatable);
            }

            public void SetBackground(Color color)
            {
                internalRenderScheduler.AddTask(mainCanvas.SetBackGround(color, 100), onRenderDone);
            }

            public void Clear()
            {
                elements.Clear();
                runtimeUpdatableElements.Clear();

                SetBackground(backgroundColor);
            }

            public IEnumerator<bool> Generate()
            {
                Task renderBackgroundTask = new Task(mainCanvas.SetBackGround(backgroundColor, 100));
                while (renderBackgroundTask.MoveNext())
                    yield return true;

                var tempElements = new HashSet<IMonoElement>(elements);

                foreach (var element in tempElements)
                {
                    yield return true;

                    Task internalRenderTask = null;

                    var complexElement = element as IComplexElement;
                    
                    if (complexElement != null && complexElement is IRuntimeUpdatable)
                    {

                        internalRenderTask = new Task(complexElement.Generate());

                        while (internalRenderTask.MoveNext())
                            yield return true;
                    }


                    internalRenderTask = new Task(mainCanvas.MergeCanvas(element.Draw(), element.Position, 100));
                    while (internalRenderTask.MoveNext())
                        yield return true;
                }

                onRenderDone?.Invoke();
            }

            public IEnumerator<bool> RenderAddedElement(IMonoElement element)
            {
                Task internalRenderTask = new Task(mainCanvas.MergeCanvas(element.Draw(), element.Position, 100));
                while (internalRenderTask.MoveNext())
                    yield return true;
            }

            public StringBuilder Draw()
            {
                return mainCanvas.ToStringBuilder();
            }

            public void RunRenderer()
            {
                internalRenderScheduler.Main();

                CheckUpdatableComponents();
            }

            private void CheckUpdatableComponents()
            {
                bool redraw = false;

                foreach (var runtimeUpdatable in runtimeUpdatableElements)
                {
                    if (runtimeUpdatable.Updated)
                    {
                        redraw = true;

                        runtimeUpdatable.Updated = false;
                        break;
                    }
                }

                if (redraw)
                    internalRenderScheduler.AddTask(Generate());
            }
        }
	}
}

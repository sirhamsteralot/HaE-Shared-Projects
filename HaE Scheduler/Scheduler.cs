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
        public class Scheduler
	    {
            public int runsPerTick;
            public int QueueCount => Queue.Count;

            Queue<Task> Queue = new Queue<Task>();

            public Scheduler(int runsPerTick = 1)
            {
                this.runsPerTick = runsPerTick;
            }

            public bool Main()
            {
                for (int i = 0; i < runsPerTick; i++)
                {
                    if (Queue.Count == 0)
                        return false;

                    var current = Queue.First();

                    if (!current.MoveNext())
                    {
                        Queue.Dequeue().Dispose();
                        current.Callback?.Invoke();
                    }
                }

                return true;
            }

            public void AddTask(Task enumerator)
            {
                Queue.Enqueue(enumerator);
            }

            public void AddTask(IEnumerator<bool> enumerator)
            {
                var task = new Task(enumerator);
                AddTask(task);
            }

            public void AddTask(IEnumerator<bool> enumerator, Action callback)
            {
                var task = new Task(enumerator, callback);
                AddTask(task);
            }
        }
	}
}

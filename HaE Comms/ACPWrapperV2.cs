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
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public class ACPWrapperV2
        {
            public HashSet<string> listenIds = new HashSet<string>();

            private List<Antenna> antennas;
            private GridTerminalSystemUtils GTS;

            private Queue<Message> messageQueue;

            public ACPWrapperV2(GridTerminalSystemUtils GTS, string antennaTag = null)
            {
                this.GTS = GTS;

                GTS.GridTerminalSystem.GetBlocksOfType<IMyFunctionalBlock>(null,
                    x =>
                    {
                        if (antennaTag == null)
                            return false;
                        else if (!x.CustomName.Contains(antennaTag))
                            return false;

                        var antenna = x as IMyRadioAntenna;
                        if (antenna != null)
                        {
                            antennas.Add(new Antenna(antenna));
                        }
                        else
                        {
                            var laserAntenna = x as IMyLaserAntenna;
                            if (laserAntenna != null)
                                antennas.Add(new Antenna(laserAntenna));
                        }

                        return false;
                    });

                listenIds.Add("0");
            }

            public bool TryPeelMessage(string message, ref long senderId, ref string peeledMessagePart)
            {
                string[] msgParts = message.Split('|');

                if (msgParts.Length < 3)
                    return false;

                if (!listenIds.Contains(msgParts[1]) && !(msgParts[1] == GTS.Me.EntityId.ToString()))
                    return false;

                if (!long.TryParse(msgParts[0], out senderId))
                    return false;

                peeledMessagePart = msgParts[2];

                return true;
            }

            StringBuilder messageBuilder = new StringBuilder();
            public void EnqueueMessage(string message, long targetId, MyTransmitTarget target, antennaType type)
            {
                messageBuilder.Clear();
                messageBuilder.Append(GTS.Me.EntityId);
                messageBuilder.Append('|');
                messageBuilder.Append(targetId);
                messageBuilder.Append('|');
                messageBuilder.Append(message);


                var msg = new Message
                {
                    message = messageBuilder.ToString(),
                    target = target,
                    antennaType = type,
                    isEmpty = false
                };

                messageQueue.Enqueue(msg);
            }

            public IEnumerator<bool> TaskLoop()
            {
                while (true)
                {
                    ProcessQueue();
                    yield return true;
                }
            }

            Message emptyMessage = new Message { isEmpty = true };
            Message queueItem = new Message { isEmpty = true };
            private void ProcessQueue()
            {
                foreach (var antenna in antennas)
                {
                    bool queueEmpty = messageQueue.Count < 1;
                    if (queueItem.isEmpty && !queueEmpty)
                        queueItem = messageQueue.Dequeue();
                    else if (queueEmpty)
                        return;

                    if ((antenna.type & queueItem.antennaType) != 0)
                        continue;

                    if (antenna.SendMessage(queueItem))
                        queueItem = emptyMessage;
                }
            }


            private struct Message
            {
                public string message;
                public antennaType antennaType;
                public MyTransmitTarget target;

                public bool isEmpty;
            }

            private struct Antenna
            {
                public antennaType type;

                public IMyLaserAntenna laserAntenna;
                public IMyRadioAntenna radioAntenna;

                public Antenna(IMyRadioAntenna radioAntenna) : this()
                {
                    if (radioAntenna == null)
                        throw new Exception("radioAntenna cannot be null!");

                    this.radioAntenna = radioAntenna;
                    type = antennaType.radioAntenna;
                }

                public Antenna(IMyLaserAntenna laserAntenna) : this()
                {
                    if (laserAntenna == null)
                        throw new Exception("laserAntenna cannot be null!");

                    this.laserAntenna = laserAntenna;
                    type = antennaType.laserAntenna;
                }

                public bool SendStringMessage(string message, MyTransmitTarget target = MyTransmitTarget.Default)
                {
                    if (type == antennaType.radioAntenna)
                    {
                        return radioAntenna.TransmitMessage(message, target);
                    } else
                    {
                        if (laserAntenna.Status == MyLaserAntennaStatus.Connected)
                            return laserAntenna.TransmitMessage(message);
                    }

                    return false;
                }

                public bool SendMessage(Message message)
                {
                    return SendStringMessage(message.message, message.target);
                }
            }

            public enum antennaType
            {
                radioAntenna,
                laserAntenna
            }
        }
    }
}

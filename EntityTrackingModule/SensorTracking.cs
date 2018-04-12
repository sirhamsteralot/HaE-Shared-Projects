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
        public class SensorTracking : ITracking
        {
            public Action<HaE_Entity> OnEntityDetected { get; set; }

            private HashSet<IMySensorBlock> sensors;
            private const HaE_Entity.TrackingType TRACKINGTYPE = HaE_Entity.TrackingType.Sensor;

            public SensorTracking(HashSet<IMySensorBlock> sensors)
            {
                this.sensors = sensors;
            }

            public SensorTracking(List<IMySensorBlock> sensors)
            {
                this.sensors = new HashSet<IMySensorBlock>(sensors);
            }

            List<MyDetectedEntityInfo> templist = new List<MyDetectedEntityInfo>();
            public void Poll()
            {
                foreach (var sensor in sensors)
                {
                    if (sensor.Enabled)
                    {
                        templist.Clear();
                        sensor.DetectedEntities(templist);

                        foreach (var entity in templist)
                        {
                            HaE_Entity detectedEntity = new HaE_Entity
                            {
                                entityInfo = entity,
                                LastDetectionTime = DateTime.Now,
                                trackingType = TRACKINGTYPE
                            };

                            OnEntityDetected?.Invoke(detectedEntity);
                        }
                    }
                }
            }
        }
	}
}

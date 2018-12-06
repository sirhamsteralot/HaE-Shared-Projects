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

            private EntityTracking_Module.refExpSettings refExpSettings = EntityTracking_Module.refExpDefault;
            private IMyProgrammableBlock Me;

            public SensorTracking(HashSet<IMySensorBlock> sensors)
            {
                this.sensors = sensors;
            }

            public SensorTracking(List<IMySensorBlock> sensors)
            {
                this.sensors = new HashSet<IMySensorBlock>(sensors);
            }

            public void SetRefExpSettings(IMyProgrammableBlock Me, EntityTracking_Module.refExpSettings refExpSettings)
            {
                this.refExpSettings = refExpSettings;
                this.Me = Me;
            }

            List<MyDetectedEntityInfo> templist = new List<MyDetectedEntityInfo>();
            public void Poll()
            {
                foreach (var sensor in sensors)
                {
                    if (sensor.Enabled)
                    {
                        if ((refExpSettings & EntityTracking_Module.refExpSettings.Lidar) != 0 || (Me?.IsSameConstructAs(sensor) ?? true))
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
}

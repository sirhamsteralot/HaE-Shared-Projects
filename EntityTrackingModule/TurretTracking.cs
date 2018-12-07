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
        public class TurretTracking : ITracking
	    {
            public Action<HaE_Entity> OnEntityDetected { get; set; }

            private HashSet<IMyLargeTurretBase> turrets;
            private const HaE_Entity.TrackingType TRACKINGTYPE = HaE_Entity.TrackingType.Turret;

            private EntityTracking_Module.refExpSettings refExpSettings = EntityTracking_Module.refExpDefault;
            private IMyProgrammableBlock Me;

            public TurretTracking(HashSet<IMyLargeTurretBase> turrets)
            {
                this.turrets = turrets;
            }

            public TurretTracking(List<IMyLargeTurretBase> turrets) : this(new HashSet<IMyLargeTurretBase>(turrets))
            {
            }

            public void SetRefExpSettings(IMyProgrammableBlock Me, EntityTracking_Module.refExpSettings refExpSettings)
            {
                this.refExpSettings = refExpSettings;
                this.Me = Me;
            }

            public void TimeOutEntities(TimeSpan timeoutTime) { }
            public void ClearTrackedEntities() { }

            public void Poll()
            {
                foreach (var turret in turrets)
                {
                    if (turret.HasTarget)
                    {
                        if ((refExpSettings & EntityTracking_Module.refExpSettings.Turret) != 0 || (Me?.IsSameConstructAs(turret) ?? true))
                        {
                            HaE_Entity detectedEntity = new HaE_Entity
                            {
                                entityInfo = turret.GetTargetedEntity(),
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

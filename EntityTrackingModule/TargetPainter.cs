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
        public class TargetPainter
	    {
            private IMyCameraBlock targetingCamera;
            private List<IMyCameraBlock> cameras;
            private const HaE_Entity.TrackingType TRACKINGTYPE = HaE_Entity.TrackingType.Lidar;

            private EntityTracking_Module.refExpSettings refExpSettings = EntityTracking_Module.refExpDefault;
            private IMyProgrammableBlock Me;

            public TargetPainter(IMyCameraBlock targetingCamera, List<IMyCameraBlock> cameras)
            {
                this.targetingCamera = targetingCamera;
                this.cameras = cameras;
            }

            public void SetRefExpSettings(HashSet<IMyLargeTurretBase> turrets, IMyProgrammableBlock Me, EntityTracking_Module.refExpSettings refExpSettings)
            {
                this.refExpSettings = refExpSettings;
                this.Me = Me;
            }

            public HaE_Entity PaintTarget(double distance)
            {
                if (targetingCamera == null)
                    return null;

                MyDetectedEntityInfo detected = targetingCamera.Raycast(distance);
                if (!detected.IsEmpty())
                {
                    HaE_Entity detectedEntity = new HaE_Entity
                    {
                        entityInfo = detected,
                        LastDetectionTime = DateTime.Now,
                        trackingType = TRACKINGTYPE
                    };

                    return detectedEntity;
                }

                return null;
            }

            public HaE_Entity PaintTarget(Vector3D position)
            {
                foreach (var camera in cameras)
                {
                    if ((refExpSettings & EntityTracking_Module.refExpSettings.Lidar) != 0 || camera.IsSameConstructAs(Me))
                    {
                        if (camera.CanScan(position))
                        {
                            MyDetectedEntityInfo detected = camera.Raycast(position);

                            HaE_Entity detectedEntity = new HaE_Entity
                            {
                                entityInfo = detected,
                                LastDetectionTime = DateTime.Now,
                                trackingType = TRACKINGTYPE
                            };

                            return detectedEntity;
                        }
                    }
                }
                return null;
            }
        }
	}
}

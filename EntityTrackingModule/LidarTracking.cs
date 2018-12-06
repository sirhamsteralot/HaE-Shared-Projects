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
        public class LidarTracking : ITracking
        {
            public Action<HaE_Entity> OnEntityDetected { get; set; }
            public int MaxReScanAmount = 10;
            public double ReScanMultiplier = 1.05;
            public double ReScanConeAngle = 0.0125006511;

            private HashSet<IMyCameraBlock> cameras;
            private const HaE_Entity.TrackingType TRACKINGTYPE = HaE_Entity.TrackingType.Lidar;
            private HashSet<HaE_Entity> trackedEntities;
            private IMyTerminalBlock reference;
            private Random random = new Random();

            private IEnumerator<bool> _castingSpreader;

            private EntityTracking_Module.refExpSettings refExpSettings = EntityTracking_Module.refExpDefault;
            private IMyProgrammableBlock Me;


            public LidarTracking(HashSet<IMyCameraBlock> cameras, IMyTerminalBlock reference, HashSet<HaE_Entity> trackedEntities)
            {
                this.cameras = cameras;
                this.trackedEntities = trackedEntities;
                this.reference = reference;

                CommonInit();
            }
            public LidarTracking(List<IMyCameraBlock> cameras, IMyTerminalBlock reference, HashSet<HaE_Entity> trackedEntities)
            {
                this.cameras = new HashSet<IMyCameraBlock>(cameras);
                this.trackedEntities = trackedEntities;
                this.reference = reference;

                CommonInit();
            }

            public void SetRefExpSettings(IMyProgrammableBlock Me, EntityTracking_Module.refExpSettings refExpSettings)
            {
                this.refExpSettings = refExpSettings;
                this.Me = Me;
            }

            private void CommonInit()
            {
                foreach (var camera in cameras)
                {
                    camera.EnableRaycast = true;
                }
            }

            public void Poll()
            {
                if (_castingSpreader != null)
                {
                    if (!_castingSpreader.MoveNext())
                    {
                        _castingSpreader.Dispose();
                        _castingSpreader = CastingSpreader();
                    }
                }
                else
                {
                    _castingSpreader = CastingSpreader();
                }
            }

            private IEnumerator<bool> CastingSpreader()
            {
                var Templist = new HashSet<HaE_Entity>(trackedEntities);

                foreach (var entity in Templist)
                {
                    RaycastEntity(entity);
                    yield return true;
                }
            }

            private void RaycastEntity(HaE_Entity entity)
            {
                Vector3D extrapolatedPosition = TrackingUtils.GetPredictedEntityLocation(entity);
                HaE_Entity detectedEntity = RaycastPosition(extrapolatedPosition);

                if (detectedEntity == null)
                {
                    if (!AttemptReScan(entity))
                        return;
                }

                OnEntityDetected?.Invoke(detectedEntity);
            }


            private bool AttemptReScan(HaE_Entity entity)
            {
                Vector3D Direction = entity.entityInfo.Position - reference.GetPosition();
                double distance = Direction.Normalize();

                for (int i = 0; i < MaxReScanAmount; i++)
                {
                    Vector3D scanDir = TrackingUtils.GenerateShotgunVector(random, Direction, ReScanConeAngle);
                    HaE_Entity detected = RaycastDirection(Direction, distance * ReScanMultiplier);

                    if (detected != null)
                    {
                        OnEntityDetected?.Invoke(detected);
                        return true;
                    }
                }

                return false;
            }

            public HaE_Entity RaycastDirection (Vector3D direction, double distance)
            {
                Vector3D position = reference.GetPosition() + direction * distance;
                IMyCameraBlock camera = GetUsableCamera(position);
                if (camera == null)
                    return null;

                MyDetectedEntityInfo detected = camera.Raycast(position);
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

            public HaE_Entity RaycastPosition (Vector3D position)
            {
                IMyCameraBlock camera = GetUsableCamera(position);
                if (camera == null)
                    return null;


                MyDetectedEntityInfo detected = camera.Raycast(position);
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

            private IMyCameraBlock GetUsableCamera(Vector3D raycastPos)
            {
                foreach (var camera in cameras)
                {
                    if ((refExpSettings & EntityTracking_Module.refExpSettings.Lidar) != 0 || (Me?.IsSameConstructAs(camera) ?? true))
                    {
                        if (camera.CanScan(raycastPos))
                        {
                            return camera;
                        }
                    }
                }

                return null;
            }

            private IMyCameraBlock GetUsableCamera(double raycastDist)
            {
                foreach (var camera in cameras)
                {
                    if ((refExpSettings & EntityTracking_Module.refExpSettings.Lidar) != 0 || (Me?.IsSameConstructAs(camera) ?? true))
                    {
                        if (camera.CanScan(raycastDist))
                        {
                            return camera;
                        }
                    }
                }

                return null;
            }
        }
    }
}

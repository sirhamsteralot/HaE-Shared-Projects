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
        public class EntityTracking_Module
	    {
            public const refExpSettings refExpDefault = refExpSettings.Lidar | refExpSettings.Sensor | refExpSettings.Turret;
            public Known_Objects known_Objects = new Known_Objects();
            public List<ITracking> ObjectTrackers = new List<ITracking>();
            public Action<HaE_Entity> onEntityDetected;

            private TargetPainter targetPainter;
            private refExpSettings supportRefExp = refExpDefault;

            public EntityTracking_Module(GridTerminalSystemUtils GTS, IMyShipController reference, IMyCameraBlock targetingCamera, refExpSettings supportRefExp, string ignoreTag = null) : this(GTS, reference, targetingCamera, ignoreTag)
            {
                this.supportRefExp = supportRefExp;

                foreach(var tracker in ObjectTrackers)
                {
                    tracker.SetRefExpSettings(GTS.Me, supportRefExp);
                }

                targetPainter.SetRefExpSettings(GTS.Me, supportRefExp);
            }

            public EntityTracking_Module(GridTerminalSystemUtils GTS, IMyShipController reference, IMyCameraBlock targetingCamera, string ignoreTag = null)
            {
                Func<IMyTerminalBlock, bool> filter = null;
                if (ignoreTag != null)
                {
                    filter = (x => !x.CustomName.Contains(ignoreTag));
                }

                List<IMyLargeTurretBase> turretList = new List<IMyLargeTurretBase>();
                GTS.GridTerminalSystem.GetBlocksOfType(turretList, filter);
                ObjectTrackers.Add(new TurretTracking(turretList));

                List<IMySensorBlock> sensorList = new List<IMySensorBlock>();
                GTS.GridTerminalSystem.GetBlocksOfType(sensorList, filter);
                ObjectTrackers.Add(new SensorTracking(sensorList));

                List<IMyCameraBlock> cameraList = new List<IMyCameraBlock>();
                GTS.GridTerminalSystem.GetBlocksOfType(cameraList, filter);
                ObjectTrackers.Add(new LidarTracking(cameraList, reference, known_Objects.LidarEntities));

                targetPainter = new TargetPainter(targetingCamera, cameraList);

                foreach (var tracker in ObjectTrackers)
                {
                    tracker.OnEntityDetected += OnEntityDetected;
                }
            }

            public void PaintTarget(double distance)
            {
                var entity = targetPainter.PaintTarget(distance);
                OnEntityDetected(entity);
            }

            public HaE_Entity PaintTarget(Vector3D pos)
            {

                var entity = targetPainter.PaintTarget(pos);
                OnEntityDetected(entity);

                return entity;
            }

            public void Poll()
            {
                foreach (var tracker in ObjectTrackers)
                {
                    tracker.Poll();
                }
            }

            public void TimeoutEntities(TimeSpan timeout)
            {
                known_Objects.TimeOutEntities(timeout);
            }

            public void ClearEntities()
            {
                known_Objects.Clear();
                foreach (var tracker in ObjectTrackers)
                {
                    tracker.ClearTrackedEntities();
                }
            }

            private void OnEntityDetected(HaE_Entity entity)
            {
                if (entity == null)
                    return;

                if (known_Objects.Contains(entity))
                {
                    known_Objects.UpdateEntity(entity);
                }
                else
                {
                    known_Objects.AddEntity(entity);
                }

                onEntityDetected?.Invoke(entity);
            }

            [Flags]
            public enum refExpSettings
            {
                Lidar,
                Turret,
                Sensor,
                OwnGrid,
                None,
            }
	    }
	}
}

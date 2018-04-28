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
            public Known_Objects known_Objects = new Known_Objects();
            public List<ITracking> ObjectTrackers = new List<ITracking>();
            private TargetPainter targetPainter;

            public EntityTracking_Module(GridTerminalSystemUtils GTS, IMyShipController reference, IMyCameraBlock targetingCamera)
            {
                

                List<IMyLargeTurretBase> turretList = new List<IMyLargeTurretBase>();
                GTS.GridTerminalSystem.GetBlocksOfType(turretList);
                ObjectTrackers.Add(new TurretTracking(turretList));

                List<IMySensorBlock> sensorList = new List<IMySensorBlock>();
                GTS.GridTerminalSystem.GetBlocksOfType(sensorList);
                ObjectTrackers.Add(new SensorTracking(sensorList));

                List<IMyCameraBlock> cameraList = new List<IMyCameraBlock>();
                GTS.GridTerminalSystem.GetBlocksOfType(cameraList);
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
            }
	    }
	}
}

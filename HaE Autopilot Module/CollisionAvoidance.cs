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
        public class CollisionAvoidance
	    {
            private List<HaE_Entity> relevantObjects = new List<HaE_Entity>();
            private BoundingSphereD boundingSphere;

            private List<Vector3D> localScanMap;

            private IMyShipController rc;
            private EntityTracking_Module trackingModule;

            public CollisionAvoidance(IMyShipController rc, EntityTracking_Module trackingModule, int resX, int resY)
            {
                this.rc = rc;
                this.trackingModule = trackingModule;

                localScanMap = BuildScanMap(resX, resY);
            }

            public void RegisterObject(HaE_Entity entity)
            {
                relevantObjects.Add(entity);
                boundingSphere = BoundingSphereD.CreateMerged(boundingSphere, entity.BoundingSphere);
            }

            public void UnRegisterObject(HaE_Entity entity)
            {
                relevantObjects.Remove(entity);
                CreateSphereFromEntities();
            }

            public void ClearEntities()
            {
                relevantObjects.Clear();
                ResetBoundingSphere();
            }

            public bool NextPosition(out Vector3D vector)
            {

            }

            public bool Scan(double distance)
            {
                bool detected = false;
                foreach(var direction in localScanMap)
                {
                    Vector3D worldDirection = VectorUtils.TransformDirLocalToWorld(rc.WorldMatrix, direction);
                    Vector3D worldPos = rc.GetPosition() + worldDirection * distance;

                    HaE_Entity entity = trackingModule.PaintTarget(worldPos);
                    if (entity != null)
                    {
                        RegisterObject(entity);
                        detected = true;
                    }
                }

                return detected;
            }

            private List<Vector3D> BuildScanMap(int resX, int resY)
            {
                double multiplierX = 1.0 / resX;
                double multiplierY = 1.0 / resY;

                Vector3D localUp = Vector3D.Up;
                Vector3D localLeft = Vector3D.Left;
                Vector3D localForward = Vector3D.Forward;

                Vector3D beginFlat = localForward - localLeft;

                var scanMap = new List<Vector3D>();

                for (int x = 0; x < resX; x++)
                {
                    Vector3D flatX = Vector3D.Normalize(beginFlat + localLeft * (multiplierX * x));
                    Vector3D bottomX = Vector3D.Normalize(flatX - localUp);

                    for (int y = 0; y < resY; y++)
                    {
                        Vector3D XY = Vector3D.Normalize(bottomX + localUp * (multiplierY * y));
                        scanMap.Add(XY);
                    }
                }

                return scanMap;
            }

            private void CreateSphereFromEntities()
            {
                ResetBoundingSphere();

                foreach (var entity in relevantObjects)
                {
                    boundingSphere = BoundingSphereD.CreateMerged(boundingSphere, entity.BoundingSphere);
                }
            }

            private void ResetBoundingSphere()
            {
                boundingSphere = default(BoundingSphereD);
            }
	    }
	}
}

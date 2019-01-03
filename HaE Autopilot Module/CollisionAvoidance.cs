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
            private Dictionary<long, HaE_Entity> relevantEntities = new Dictionary<long, HaE_Entity>();
            private BoundingSphereD boundingSphere;

            private List<Vector3D> localScanMap;

            private IMyShipController rc;
            private EntityTracking_Module trackingModule;

            private Vector3D lastHeadingDir;

            public CollisionAvoidance(IMyShipController rc, EntityTracking_Module trackingModule, int resX, int resY)
            {
                this.rc = rc;
                this.trackingModule = trackingModule;

                localScanMap = BuildScanMap(resX, resY);
            }

            
            public void OnEntityDetected(HaE_Entity entity)
            {
                if (CheckIfEntityRelevant(entity, lastHeadingDir))
                {
                    relevantEntities[entity.entityInfo.EntityId] = entity;
                }
            }

            public bool CheckForObjects()
            {
                return relevantEntities.Count > 0;
            }

            public void NextPosition(ref Vector3D nextDirection, Vector3D headingDir, double safetyMargin = 1.25)
            {
                CreateSphereFromEntities(headingDir);

                Vector3D position = rc.CubeGrid.WorldVolume.Center;
                Vector3D movementDir = rc.GetShipVelocities().LinearVelocity;
                movementDir.Normalize();

                RayD movementRay = new RayD(position, movementDir);
                RayD headingRay = new RayD(rc.CubeGrid.WorldVolume.Center, headingDir);

                BoundingSphereD sphere = boundingSphere;
                sphere.Radius += rc.CubeGrid.WorldVolume.Radius;

                if (sphere.Contains(position) != ContainmentType.Disjoint)
                {
                    Vector3D dodgeDirection = Vector3D.Normalize(position - sphere.Center);

                    nextDirection = sphere.Center + dodgeDirection * sphere.Radius * safetyMargin;
                    return;
                }

                double? movementDist = sphere.Intersects(movementRay);
                double? headingDist = sphere.Intersects(headingRay);

                if (movementDist.HasValue || headingDist.HasValue)
                {
                    Vector3D pointOnSphere;
                    Vector3D dodgeDirection;
                    if (movementDist.HasValue)
                    {
                        pointOnSphere = position + movementDir * movementDist.Value;
                        dodgeDirection = GetAvoidanceVector(pointOnSphere, sphere, movementDir);
                    }   
                    else
                    {
                        pointOnSphere = position + headingDir * headingDist.Value;
                        dodgeDirection = GetAvoidanceVector(pointOnSphere, sphere, headingDir);
                    }

                    nextDirection = dodgeDirection;
                }
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
                        detected = true;
                    }
                }

                return detected;
            }

            private Vector3D GetAvoidanceVector(Vector3D sphereContactPoint, BoundingSphereD sphere, Vector3D travelDir)
            {
                Vector3D tangentPlaneNormal = sphereContactPoint - sphere.Center;
                Vector3D avoidanceVector = Vector3D.Normalize(VectorUtils.Reject(tangentPlaneNormal, travelDir));

                return avoidanceVector;
            }

            private bool CheckIfEntityRelevant(HaE_Entity entity, Vector3D currentHeading)
            {
                if (entity.entityInfo.EntityId == rc.CubeGrid.EntityId)
                    return false;

                if (entity.BoundingSphere.Radius <= 0)
                    return false;

                Vector3D movementDir = rc.GetShipVelocities().LinearVelocity;
                movementDir.Normalize();

                RayD movementRay = new RayD(rc.CubeGrid.WorldVolume.Center, movementDir);
                RayD headingRay = new RayD(rc.CubeGrid.WorldVolume.Center, currentHeading);

                BoundingSphereD sphere = entity.BoundingSphere;
                sphere.Radius += rc.CubeGrid.WorldVolume.Radius;

                double? intersectVel = sphere.Intersects(movementRay);
                double? intersectHeading = sphere.Intersects(headingRay);

                return intersectVel.HasValue || intersectHeading.HasValue;
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

            private HashSet<long> idsToRemove = new HashSet<long>();
            private void CreateSphereFromEntities(Vector3D headingDir)
            {
                ResetBoundingSphere();

                foreach (var entity in relevantEntities.Values)
                {
                    if (CheckIfEntityRelevant(entity, headingDir))
                    {
                        boundingSphere.Include(entity.BoundingSphere);
                    }
                    else
                    {
                        idsToRemove.Add(entity.entityInfo.EntityId);
                    }
                }

                foreach (var id in idsToRemove)
                {
                    relevantEntities.Remove(id);
                }
            }

            private void ResetBoundingSphere()
            {
                boundingSphere = default(BoundingSphereD);
            }
	    }
	}
}

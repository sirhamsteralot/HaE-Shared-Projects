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
        public class Known_Objects : ICollection
	    {
            private HashSet<HaE_Entity> detectedEntitiesSet;
            private Dictionary<long, HaE_Entity> detectedEntitiesDict;

            private HashSet<HaE_Entity> _turretEntities;
            public HashSet<HaE_Entity> TurretEntities => _turretEntities;

            private HashSet<HaE_Entity> _sensorEntities;
            public HashSet<HaE_Entity> SensorEntities => _sensorEntities;

            private HashSet<HaE_Entity> _lidarEntities;
            public HashSet<HaE_Entity> LidarEntities => _lidarEntities;

            public Known_Objects()
            {
                detectedEntitiesSet  = new HashSet<HaE_Entity>(new EntityInfoComparer());
                detectedEntitiesDict = new Dictionary<long, HaE_Entity>();

                _turretEntities = new HashSet<HaE_Entity>(new EntityInfoComparer());
                _sensorEntities = new HashSet<HaE_Entity>(new EntityInfoComparer());
                _lidarEntities  = new HashSet<HaE_Entity>(new EntityInfoComparer());
            }

            public int Count => ((ICollection)detectedEntitiesSet).Count;

            public object SyncRoot => ((ICollection)detectedEntitiesSet).SyncRoot;

            public bool IsSynchronized => ((ICollection)detectedEntitiesSet).IsSynchronized;

            public void CopyTo(Array array, int index)
            {
                ((ICollection)detectedEntitiesSet).CopyTo(array, index);
            }

            public IEnumerator GetEnumerator()
            {
                return detectedEntitiesSet.GetEnumerator();
            }

            public bool GetEntityById (long entityId, ref HaE_Entity entity)
            {
                if (!detectedEntitiesDict.ContainsKey(entityId))
                    return false;

                entity = detectedEntitiesDict[entityId];
                return true;
            }

            public bool UpdateEntity(HaE_Entity entity)
            {
                if (!detectedEntitiesDict.ContainsKey(entity.entityInfo.EntityId))
                    return false;

                detectedEntitiesDict[entity.entityInfo.EntityId] = entity;

                detectedEntitiesSet.Remove(entity);
                detectedEntitiesSet.Add(entity);

                switch (entity.trackingType)
                {
                    case HaE_Entity.TrackingType.Lidar:
                        _lidarEntities.Remove(entity);
                        _lidarEntities.Add(entity);
                        break;
                    case HaE_Entity.TrackingType.Sensor:
                        _sensorEntities.Remove(entity);
                        _sensorEntities.Add(entity);
                        break;
                    case HaE_Entity.TrackingType.Turret:
                        _turretEntities.Remove(entity);
                        _turretEntities.Add(entity);
                        break;
                }

                return true;
            }

            public bool AddEntity(HaE_Entity entity)
            {
                if (detectedEntitiesDict.ContainsKey(entity.entityInfo.EntityId))
                    return false;

                detectedEntitiesDict[entity.entityInfo.EntityId] = entity;
                detectedEntitiesSet.Add(entity);

                switch (entity.trackingType)
                {
                    case HaE_Entity.TrackingType.Lidar:
                        _lidarEntities.Add(entity);
                        break;
                    case HaE_Entity.TrackingType.Sensor:
                        _sensorEntities.Add(entity);
                        break;
                    case HaE_Entity.TrackingType.Turret:
                        _turretEntities.Add(entity);
                        break;
                }

                return true;
            }

            public void AddRange(IEnumerable<HaE_Entity> entities)
            {
                foreach (var entity in entities)
                {
                    AddEntity(entity);
                }
            }

            public bool Contains(HaE_Entity entity)
            {
                return detectedEntitiesSet.Contains(entity);
            }

            public bool Remove(HaE_Entity entity)
            {
                if (detectedEntitiesDict.Remove(entity.entityInfo.EntityId))
                {
                    _lidarEntities.Remove(entity);
                    _sensorEntities.Remove(entity);
                    _turretEntities.Remove(entity);
                    detectedEntitiesSet.Remove(entity);
                    return true;
                }

                return false;
            }

            public void TimeOutEntities(TimeSpan timeoutTime)
            {
                var TempSet = new HashSet<HaE_Entity>(detectedEntitiesSet);

                foreach (var entity in TempSet)
                {
                    if (entity.GetTimeSinceAdded() > timeoutTime)
                        Remove(entity);
                }
            }

            public void Clear()
            {
                detectedEntitiesDict.Clear();
                _lidarEntities.Clear();
                _sensorEntities.Clear();
                _turretEntities.Clear();
                detectedEntitiesSet.Clear();
            }

            StringBuilder sb = new StringBuilder();
            StringBuilder throwAwaySb = new StringBuilder();
            public override string ToString()
            {
                sb.Clear();
                throwAwaySb.Clear();

                foreach(var entity in detectedEntitiesSet)
                {
                    sb.AppendLine(entity.ToString(throwAwaySb));
                    sb.AppendLine("==========================================================");
                }

                return sb.ToString();
            }

            private class EntityInfoComparer : IEqualityComparer<HaE_Entity>
            {
                public bool Equals(HaE_Entity x, HaE_Entity y)
                {
                    return x.entityInfo.EntityId == y.entityInfo.EntityId;
                }

                public int GetHashCode(HaE_Entity obj)
                {
                    return obj.entityInfo.EntityId.GetHashCode();
                }
            }
        }
	}
}

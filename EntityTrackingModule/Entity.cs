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
        public class HaE_Entity : IEquatable<HaE_Entity>
        {
            public MyDetectedEntityInfo entityInfo;
            public DateTime LastDetectionTime;
            public TrackingType trackingType;

            public BoundingSphereD BoundingSphere => BoundingSphereD.CreateFromBoundingBox(entityInfo.BoundingBox);

            public bool Equals(HaE_Entity other)
            {
                return this.entityInfo.EntityId == other.entityInfo.EntityId;
            }

            public int GetTicksSinceAdded()
            {
                return (int)Math.Round((DateTime.Now - LastDetectionTime).TotalMilliseconds / 0.01666666666);
            }

            public TimeSpan GetTimeSinceAdded()
            {
                return DateTime.Now - LastDetectionTime;
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                return ToString(sb);
            }

            public string ToString(StringBuilder sb)
            {
                sb.Clear();

                sb.Append("Name: ").Append(entityInfo.Name);
                sb.Append("\nId: ").Append(entityInfo.EntityId);
                sb.Append("\nType: ").Append(trackingType.ToString());
                sb.Append("\nPosition: ").Append(entityInfo.Position);
                sb.Append("\nLastDetected: ").Append(LastDetectionTime.ToString("HH:mm:ss.ffff"));

                return sb.ToString();
            }

            public static explicit operator MyDetectedEntityInfo(HaE_Entity x)
            {
                return x.entityInfo;
            }

            public enum TrackingType
            {
                Turret,
                Sensor,
                Lidar
            };
        }
    }
}

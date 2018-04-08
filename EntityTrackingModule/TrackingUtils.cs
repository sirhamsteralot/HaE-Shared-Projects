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
        public static class TrackingUtils
	    {
            public static Vector3D GetPredictedEntityLocation(HaE_Entity entity)
            {
                Vector3D oldPosition = entity.entityInfo.Position;
                Vector3D velocity = entity.entityInfo.Velocity;
                double timeSinceLastDetectionSeconds = entity.GetTimeSinceAdded().TotalSeconds;


                return oldPosition + velocity * timeSinceLastDetectionSeconds;
            }

            public static void GenerateShotgunVectors(ref HashSet<Vector3D> ShotGunVectors, Vector3D sourceDirection, double coneAngle, int amount)
            {
                if (amount < 1)
                    return;

                Random random = new Random();
                for (int i = 0; i < amount; i++)
                {
                    ShotGunVectors.Add(GenerateShotgunVector(random, sourceDirection, coneAngle));
                }
            }

            public static Vector3D GenerateShotgunVector(Random random, Vector3D sourceDirection, double coneAngle)
            {
                Vector3D crossVec = Vector3D.Normalize(Vector3D.Cross(sourceDirection, Vector3D.Right));
                if (crossVec.Length() == 0)
                {
                    crossVec = Vector3D.Normalize(Vector3D.Cross(sourceDirection, Vector3D.Up));
                }

                double s = random.NextDouble();
                double r = random.NextDouble();

                double h = Math.Cos(coneAngle);

                double phi = 2 * Math.PI * s;

                double z = h + (1 - h) * r;
                double sinT = Math.Sqrt(1 - z * z);
                double x = Math.Cos(phi) * sinT;
                double y = Math.Sin(phi) * sinT;

                return Vector3D.Normalize(Vector3D.Multiply(Vector3D.Right, x) + Vector3D.Multiply(crossVec, y) + Vector3D.Multiply(sourceDirection, z));
            }
	    }
	}
}

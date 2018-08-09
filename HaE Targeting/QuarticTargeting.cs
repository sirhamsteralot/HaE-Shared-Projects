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
        public class QuarticTargeting
	    {
            private Vector3D _projectileVelocity;
            private Vector3D _myVelocity;
            private Vector3D _myPosition;
            private double _projectileMaxVelocity;

            public QuarticTargeting(Vector3D MyVelocity, Vector3D MyPosition, double ProjectileVelocity)
            {
                _myVelocity = MyVelocity;
                _myPosition = MyPosition;
                _projectileMaxVelocity = ProjectileVelocity;
            }

            public void UpdateValues(Vector3D MyVelocity, Vector3D MyPosition, double ProjectileVelocity)
            {
                _myVelocity = MyVelocity;
                _myPosition = MyPosition;
                _projectileMaxVelocity = ProjectileVelocity;
            }


            /// <summary>
            /// Calculates possible intersection point of projectile and target
            /// </summary>
            /// <param name="target"></param>
            /// <returns></returns>
            public Vector3D? CalculateTrajectory(MyDetectedEntityInfo target)
            {
                var relVel = target.Velocity - _myVelocity;
                var P0 = target.Position;
                var V0 = Vector3D.Normalize(relVel);
                var s0 = relVel.Length();

                if (s0 < 0.01)
                    return P0;

                var P1 = _myPosition;
                var s1 = _projectileMaxVelocity;

                //Yo dawg i heard you liked obscure calculations.
                var a = (V0.X * V0.X) + (V0.Y * V0.Y) + (V0.Z * V0.Z) - (s1 * s1);
                var b = 2 * ((P0.X * V0.X) + (P0.Y * V0.Y) + (P0.Z * V0.Z) - (P1.X * V0.X) - (P1.Y * V0.Y) - (P1.Z * V0.Z));
                var c = (P0.X * P0.X) + (P0.Y * P0.Y) + (P0.Z * P0.Z) + (P1.X * P1.X) + (P1.Y * P1.Y) + (P1.Z * P1.Z) - (2 * P1.X * P0.X) - (2 * P1.Y * P0.Y) - (2 * P1.Z * P0.Z);

                var t1 = (-b + Math.Sqrt((b * b) - (4 * a * c))) / (2 * a);
                var t2 = (-b - Math.Sqrt((b * b) - (4 * a * c))) / (2 * a);

                var t = Math.Max(t1, t2);
                if (t < 0 || double.IsNaN(t))
                    return null;

                Vector3D Intersection = P0 + V0 * s0 * t;

                return Intersection;
            }
        }
	}
}

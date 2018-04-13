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
        public static class VectorUtils
	    {
            /// project one on two
            public static Vector3D Project(Vector3D one, Vector3D two) 
            {
                Vector3D projection = one.Dot(two) / two.LengthSquared() * two;
                return projection;
            }
            
            /// calculate component of one on two
            public static double GetComponent(Vector3D one, Vector3D two)
            {
                double dotBetween = one.Dot(two);

                return one.Length() * dotBetween;
            }

            public static double GetCrossComponent(Vector3D one, Vector3D two)
            {
                Vector3D crossed = one.Cross(two);

                return crossed.Length();
            }

            /// mirror a over b
            public static Vector3D Reflect(Vector3D a, Vector3D b, double rejectionFactor = 1) 
            {
                Vector3D project_a = Project(a, b);
                Vector3D reject_a = a - project_a;
                Vector3D reflect_a = project_a - reject_a * rejectionFactor;
                return reflect_a;
            }

            /// returns angle in radians
            public static double GetAngle(Vector3D One, Vector3D Two) 
            {
                return Math.Acos(MathHelper.Clamp(One.Dot(Two) / Math.Sqrt(One.LengthSquared() * Two.LengthSquared()), -1, 1));
            }
        }
	}
}

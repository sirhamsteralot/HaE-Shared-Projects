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

            /// projects onto a plane
            public static Vector3D ProjectOnPlane(Vector3D planeLeft, Vector3D planeForward, Vector3D direction)
            {
                Vector3D normal = Vector3D.Cross(planeLeft, planeForward);
                normal.Normalize();

                Vector3D projection = direction - Project(direction, normal);

                return projection;
            }

            /// projects onto a plane
            public static Vector3D ProjectOnPlanePerpendiculair(Vector3D planeLeft, Vector3D planeForward, Vector3D direction)
            {
                Vector3D projectionLeft = Project(direction, planeLeft);
                Vector3D projectionForward = Project(direction, planeForward);

                return projectionLeft + projectionForward;
            }

            /// proejcts onto a plane
            public static Vector3D ProjectOnPlane(Vector3D planeNormal, Vector3D direction)
            {
                Vector3D projection = direction - Project(direction, planeNormal);

                return projection;
            }

            /// calculate component of one on two
            public static double GetProjectionScalar(Vector3D one, Vector3D two)
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

            public static Vector3D Reject(Vector3D a, Vector3D b)
            {
                Vector3D project_a = Project(a, b);
                Vector3D reject_a = a - project_a;
                return reject_a;
            }

            /// returns angle in radians
            public static double GetAngle(Vector3D One, Vector3D Two) 
            {
                return Math.Acos(MathHelper.Clamp(One.Dot(Two) / Math.Sqrt(One.LengthSquared() * Two.LengthSquared()), -1, 1));
            }

            public static Vector3D TransformPosLocalToWorld(MatrixD worldMatrix, Vector3D localPosition)
            {
                return Vector3D.Transform(localPosition, worldMatrix);
            }

            public static Vector3D TransformPosWorldToLocal(MatrixD worldMatrix, Vector3D worldPosition)
            {
                Vector3D referenceWorldPosition = worldMatrix.Translation;
                Vector3D worldDirection = worldPosition - referenceWorldPosition;
                return Vector3D.TransformNormal(worldDirection, MatrixD.Transpose(worldMatrix));
            }

            public static Vector3D TransformDirLocalToWorld(MatrixD worldMatrix, Vector3D localDirection)
            {
                return Vector3D.TransformNormal(localDirection, worldMatrix);
            }

            public static Vector3D TransformDirWorldToLocal(MatrixD worldMatrix, Vector3D worldDirection)
            {
                return Vector3D.TransformNormal(worldDirection, MatrixD.Transpose(worldMatrix));
            }

            public static bool IsEqual(Vector3D v1, Vector3D v2, double tolerance)
            {
                return (v1 - v2).LengthSquared() < tolerance;
            }

            private static Vector3D Quadratic(Vector3D vector)
            {
                return vector * vector;
            }

            private static Vector3D Sqrt(Vector3D vector)
            {
                return new Vector3D(Math.Sqrt(vector.X), Math.Sqrt(vector.Y), Math.Sqrt(vector.Z));
            }
        }
	}
}

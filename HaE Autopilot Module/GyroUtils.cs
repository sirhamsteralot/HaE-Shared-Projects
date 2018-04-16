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
        public static class GyroUtils
        {

            public static void PointInDirection(List<IMyGyro> gyros, IMyShipController reference, Vector3D direction, double multiplier = 1)
            {
                PointInDirection(gyros, reference.WorldMatrix, direction, multiplier);
            }

            public static void PointInDirection(List<IMyGyro> gyros, IMyShipController reference, Vector3D direction, Vector3D up, double multiplier = 1)
            {
                PointInDirection(gyros, reference.WorldMatrix, direction, up, multiplier);
            }

            public static void PointInDirection(List<IMyGyro> gyros, MatrixD reference, Vector3D direction, double multiplier = 1)
            {
                double yaw, pitch;

                Vector3D relativeDirection = Vector3D.TransformNormal(direction, Matrix.Transpose(reference));

                DirectionToPitchYaw(reference.Forward, reference.Left, reference.Up, direction, out yaw, out pitch);

                ApplyGyroOverride(gyros, reference, pitch * multiplier, yaw * multiplier, 0);
            }

            public static void PointInDirection(List<IMyGyro> gyros, MatrixD reference, Vector3D direction, Vector3D up, double multiplier = 1)
            {
                double yaw, pitch, roll;

                Vector3D relativeDirection = Vector3D.TransformNormal(direction, Matrix.Transpose(reference));
                if (up != Vector3D.Zero)
                    DirectionToPitchYawRoll(reference, direction, up, out yaw, out pitch, out roll);
                else
                    DirectionToPitchYaw(reference, direction, out yaw, out pitch, out roll);

                ApplyGyroOverride(gyros, reference, pitch * multiplier, yaw * multiplier, roll * multiplier);
            }

            public static void ApplyGyroOverride(List<IMyGyro> gyros, MatrixD reference, double pitch, double yaw, double roll)
            {
                Vector3D localRotation = new Vector3D(-pitch, yaw, roll);

                Vector3D relativeRotation = Vector3D.TransformNormal(localRotation, reference);

                foreach (IMyGyro gyro in gyros)
                {
                    Vector3D gyroRotation = Vector3D.TransformNormal(relativeRotation, Matrix.Transpose(gyro.WorldMatrix));

                    gyro.Pitch = (float)gyroRotation.X;
                    gyro.Yaw = (float)gyroRotation.Y;
                    gyro.Roll = (float)gyroRotation.Z;

                    gyro.GyroOverride = true;
                }
            }

            public static void ApplyGyroOverride(List<IMyGyro> gyros, IMyShipController reference, double pitch, double yaw, double roll)
            {
                Vector3D localRotation = new Vector3D(-pitch, yaw, roll);

                Vector3D relativeRotation = Vector3D.TransformNormal(localRotation, reference.WorldMatrix);

                foreach (IMyGyro gyro in gyros)
                {
                    Vector3D gyroRotation = Vector3D.TransformNormal(relativeRotation, Matrix.Transpose(gyro.WorldMatrix));

                    gyro.Pitch = (float)gyroRotation.X;
                    gyro.Yaw = (float)gyroRotation.Y;
                    gyro.Roll = (float)gyroRotation.Z;

                    gyro.GyroOverride = true;
                }
            }

            private static void DirectionToPitchYaw(Vector3D forward, Vector3D left, Vector3D Up, Vector3D direction, out double yaw, out double pitch)
            {
                Vector3D projectTargetUp = VectorUtils.Project(direction, Up);
                Vector3D projTargetFrontLeft = direction - projectTargetUp;

                yaw = VectorUtils.GetAngle(forward, projTargetFrontLeft);
                pitch = VectorUtils.GetAngle(direction, projTargetFrontLeft);

                //damnit keen using left hand rule and everything smh
                yaw = -1 * Math.Sign(left.Dot(direction)) * yaw;

                //use the sign bit
                pitch = Math.Sign(Up.Dot(direction)) * pitch;

                //check if the target doesnt pull a 180 on us
                if ((pitch == 0) && (yaw == 0) && (direction.Dot(forward) < 0))
                    yaw = Math.PI;
            }

            private static void DirectionToPitchYawRoll(MatrixD currentOrientation, Vector3D direction, Vector3D upDirection, out double yaw, out double pitch, out double roll)
            {
                double pitchComponentError = VectorUtils.GetProjectionScalar(currentOrientation.Up, direction);
                double yawComponentError = VectorUtils.GetProjectionScalar(currentOrientation.Right, direction);
                double rollComponentError = VectorUtils.GetProjectionScalar(upDirection, currentOrientation.Right);


                pitch = pitchComponentError;
                yaw = yawComponentError;
                roll = rollComponentError;

                //check if the target doesnt pull a 180 on us
                if ((pitch == 0) && (yaw == 0) && (direction.Dot(currentOrientation.Forward) < 0))
                    yaw = Math.PI;
            }

            private static void DirectionToPitchYaw(MatrixD currentOrientation, Vector3D direction, out double yaw, out double pitch, out double roll)
            {
                double pitchComponentError = VectorUtils.GetProjectionScalar(currentOrientation.Up, direction);
                double yawComponentError = VectorUtils.GetProjectionScalar(currentOrientation.Right, direction);
                double rollComponentError = 0;


                pitch = pitchComponentError;
                yaw = yawComponentError;
                roll = rollComponentError;

                //check if the target doesnt pull a 180 on us
                if ((pitch == 0) && (yaw == 0) && (direction.Dot(currentOrientation.Forward) < 0))
                    yaw = Math.PI;
            }
        }
    }
}

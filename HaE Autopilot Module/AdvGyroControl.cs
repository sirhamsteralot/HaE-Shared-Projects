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
        public class AdvGyroControl
	    {
            public bool onlyUpdateOne = false;

            private PID_Controller yawPid;
            private PID_Controller pitchPid;
            private PID_Controller rollPid;

            private IngameTime ingameTime;
            private TimeSpan lastTime;

            public AdvGyroControl(PID_Controller.PIDSettings yawSettings, PID_Controller.PIDSettings pitchSettings, PID_Controller.PIDSettings rollSettings, IngameTime ingameTime)
            {
                yawPid = new PID_Controller(yawSettings);
                pitchPid = new PID_Controller(pitchSettings);
                rollPid = new PID_Controller(rollSettings);

                this.ingameTime = ingameTime;
            }

            public AdvGyroControl(PID_Controller.PIDSettings pidSettings, IngameTime ingameTime)
            {
                yawPid = new PID_Controller(pidSettings);
                pitchPid = new PID_Controller(pidSettings);
                rollPid = new PID_Controller(pidSettings);

                this.ingameTime = ingameTime;
            }

            public void PointInDirection(List<IMyGyro> gyros, IMyShipController reference, Vector3D direction, Vector3D up)
            {
                PointInDirection(gyros, reference.WorldMatrix, direction, up);
            }

            public void PointInDirection(List<IMyGyro> gyros, MatrixD reference, Vector3D direction, Vector3D up)
            {
                double yaw, pitch, roll;
                GyroUtils.DirectionToPitchYawRoll(reference, direction, up, out yaw, out pitch, out roll);

                double timeSinceLast = (ingameTime.Time - lastTime).TotalSeconds;

                lastTime = ingameTime.Time;

                yaw = yawPid.NextValue(yaw, timeSinceLast);
                pitch = pitchPid.NextValue(pitch, timeSinceLast);
                roll = rollPid.NextValue(roll, timeSinceLast);


                GyroUtils.ApplyGyroOverride(gyros, reference, pitch, yaw, roll, onlyUpdateOne);
            }

            public void PointInDirection(List<IMyGyro> gyros, IMyShipController reference, Vector3D direction)
            {
                PointInDirection(gyros, reference.WorldMatrix, direction);
            }

            public void PointInDirection(List<IMyGyro> gyros, MatrixD reference, Vector3D direction)
            {
                double yaw, pitch, roll;
                GyroUtils.DirectionToPitchYaw(reference, direction, out yaw, out pitch, out roll);

                double timeSinceLast = (ingameTime.Time - lastTime).TotalSeconds;
                lastTime = ingameTime.Time;

                yaw = pitchPid.NextValue(yaw, timeSinceLast);
                pitch = pitchPid.NextValue(pitch, timeSinceLast);
                roll = pitchPid.NextValue(roll, timeSinceLast);

                GyroUtils.ApplyGyroOverride(gyros, reference, pitch, yaw, roll, onlyUpdateOne);
            }

            public void StopRotation(List<IMyGyro> gyros, bool disableOverride = false)
            {
                foreach(var gyro in gyros)
                {
                    gyro.Yaw = 0;
                    gyro.Pitch = 0;
                    gyro.Roll = 0;

                    if (disableOverride)
                        gyro.GyroOverride = false;
                }
            }
        }
	}
}

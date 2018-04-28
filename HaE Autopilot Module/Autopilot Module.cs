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
        public class Autopilot_Module
	    {
            private IngameTime ingameTime;
            private PID_Controller thrustPidController;
            private TimeSpan lastTime;

            private PID_Controller positionHoldPidcontroller;
            private TimeSpan lastTimePosHoldPid;

            private List<IMyThrust> allThrusters = new List<IMyThrust>();
            private List<IMyGyro> gyros = new List<IMyGyro>();

            private IMyShipController controller;

            private AdvThrustControl thrustControl;
            private AdvGyroControl gyroControl;

            private MatrixD ControlMatrix => controller.WorldMatrix;
            private Vector3D ControlPosition => controller.GetPosition();
            private Vector3D ControlVelocity => controller.GetShipVelocities().LinearVelocity;
            private Vector3D ControlGravity => controller.GetNaturalGravity();
            private double ControlMass => controller.CalculateShipMass().PhysicalMass;

            public Autopilot_Module(GridTerminalSystemUtils GTS, IMyShipController controller, IngameTime ingameTime, PID_Controller.PIDSettings gyroPidSettings, PID_Controller.PIDSettings thrustPidSettings, PID_Controller.PIDSettings positionPidSettings)
            {
                GTS.GridTerminalSystem.GetBlocksOfType(allThrusters);
                GTS.GridTerminalSystem.GetBlocksOfType(gyros);

                this.controller = controller;
                this.ingameTime = ingameTime;

                thrustPidController = new PID_Controller(thrustPidSettings);
                positionHoldPidcontroller = new PID_Controller(positionPidSettings);

                gyroControl = new AdvGyroControl(gyroPidSettings, ingameTime);
                thrustControl = new AdvThrustControl(controller, allThrusters, ingameTime);
            }

            public void TravelToPosition(Vector3D position, double maximumVelocity = 100, double safetyMargin = 1.25)
            {
                Vector3D direction = position - ControlPosition;
                double distance = direction.Normalize();

                double stoppingDistance = CalculateStoppingDistance();

                Vector3D velocity = direction * maximumVelocity;

                if (distance >= stoppingDistance * safetyMargin)
                {
                    ThrustToVelocity(velocity);
                } else
                {
                    if (distance > 1)
                        ThrustToVelocity(direction);
                    else
                    {
                        ThrustToVelocity(direction * positionHoldPidcontroller.NextValue(distance, (lastTimePosHoldPid - ingameTime.Time).TotalSeconds));
                    }
                        
                }
            }


            public void ThrustToVelocity(Vector3D velocity)
            {
                Vector3D difference = velocity - ControlVelocity;
                double percent = thrustPidController.NextValue(difference.Length(), (lastTime - ingameTime.Time).TotalSeconds);
                percent = MathHelperD.Clamp(percent, 0, 1);

                thrustControl.ApplyForce(difference, percent);

                lastTime = ingameTime.Time;
            }

            public IEnumerable<bool> ThrustToVelocity(Vector3D velocity, double tolerance)
            {
                while (!VectorUtils.IsEqual(velocity, ControlVelocity, tolerance))
                {
                    ThrustToVelocity(velocity);
                    yield return true;
                }
            }

            public double CalculateStoppingDistance()
            {
                Vector3D velocityDir = ControlVelocity;
                double velocity = velocityDir.Normalize();
                double mass = ControlMass;

                double forceInDir = thrustControl.CalculateMaxForce(velocityDir);

                double deceleration = forceInDir / mass;

                double stoppingTime = velocity / deceleration;
                double stoppingDistance = stoppingTime * (velocity * 0.5);

                return stoppingDistance;
            }

            public void AimInDirection(Vector3D direction, Vector3D up, bool upDominant = false)
            {
                if (upDominant)
                {
                    Vector3D forwardVector = VectorUtils.ProjectOnPlane(up, direction);
                    forwardVector.Normalize();

                    gyroControl.PointInDirection(gyros, controller, forwardVector, up);
                } else
                {
                    Vector3D upVector = VectorUtils.ProjectOnPlane(direction, up);
                    upVector.Normalize();

                    gyroControl.PointInDirection(gyros, controller, direction, upVector);
                }
            }
        }
	}
}

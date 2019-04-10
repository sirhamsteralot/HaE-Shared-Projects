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

            private List<IMyThrust> allThrusters = new List<IMyThrust>();
            private List<IMyGyro> gyros = new List<IMyGyro>();

            public IMyShipController controller;

            public AdvThrustControl thrustControl;
            public AdvGyroControl gyroControl;
            public CollisionAvoidance collisionAvoidance;

            private MatrixD ControlMatrix => controller.WorldMatrix;
            private Vector3D ControlPosition => controller.GetPosition();
            private Vector3D ControlVelocity => controller.GetShipVelocities().LinearVelocity;
            private Vector3D ControlGravity => controller.GetNaturalGravity();
            private double ControlMass => controller.CalculateShipMass().PhysicalMass;



            public Autopilot_Module(GridTerminalSystemUtils GTS, IMyShipController controller, IngameTime ingameTime, 
                                    PID_Controller.PIDSettings gyroPidSettings, PID_Controller.PIDSettings thrustPidSettings,
                                    EntityTracking_Module trackingModule) : this(GTS, controller, ingameTime, trackingModule)
            {
                thrustPidController = new PID_Controller(thrustPidSettings);

                gyroControl = new AdvGyroControl(gyroPidSettings, ingameTime);
            }

            public Autopilot_Module(GridTerminalSystemUtils GTS, IMyShipController controller, IngameTime ingameTime,
                        EntityTracking_Module trackingModule)
            {
                GTS.GridTerminalSystem.GetBlocksOfType(allThrusters);
                GTS.GridTerminalSystem.GetBlocksOfType(gyros);

                this.controller = controller;
                this.ingameTime = ingameTime;

                thrustControl = new AdvThrustControl(controller, allThrusters, ingameTime);
                collisionAvoidance = new CollisionAvoidance(controller, trackingModule, 10, 10);
                trackingModule.onEntityDetected += collisionAvoidance.OnEntityDetected;

                PID_Controller.PIDSettings onePid = new PID_Controller.PIDSettings
                {
                    PGain = 1,
                    DerivativeGain = 0,
                    IntegralGain = 0,
                };

                thrustPidController = new PID_Controller(onePid);

                gyroControl = new AdvGyroControl(onePid, ingameTime);
            }

            int avoidanceCheckCounter = 0;
            public void TravelToPosition(Vector3D position, bool enableAvoidance, double maximumVelocity = 100, double safetyMargin = 1.25, bool turnSelf = true)
            {
                double distanceFromTargetSQ = (position - ControlPosition).LengthSquared();
                if (distanceFromTargetSQ > 1 || controller.GetShipSpeed() > 0.0001)
                {
                    if (enableAvoidance)
                    {
                        if (avoidanceCheckCounter++ > 100)
                        {
                            double scanDistance = CalculateStoppingDistance() * safetyMargin;
                            avoidanceCheckCounter = 0;

                            collisionAvoidance.Scan(scanDistance);
                        }
                        

                        if (collisionAvoidance.CheckForObjects())
                        {
                            Vector3D heading = Vector3D.Normalize(position - ControlPosition);
                            Vector3D nextDir = Vector3D.Zero;
                            collisionAvoidance.NextPosition(ref nextDir, heading);

                            ThrustToVelocity(nextDir * maximumVelocity);
                            gyroControl.StopRotation(gyros);
                            return;
                        }
                    }

                    TravelToPosition(position, maximumVelocity, safetyMargin, turnSelf);
                }
            }

            public void TravelToPosition(Vector3D position, double maximumVelocity = 100, double safetyMargin = 1.25, bool turnSelf = true)
            {
                Vector3D direction = position - ControlPosition;
                double distance = direction.Normalize();
                double stoppingDistance = CalculateStoppingDistance();

                Vector3D velocity = direction * maximumVelocity - VectorUtils.ProjectOnPlane(direction , ControlVelocity);

                

                if (distance >= stoppingDistance * safetyMargin && distance > 1)
                {
                    ThrustToVelocity(velocity);
                    if (turnSelf)
                        AimInDirection(Vector3D.Normalize(velocity), Vector3D.Normalize(ControlGravity));
                } else
                {
                    ThrustToVelocity(Vector3D.Zero);
                    if (turnSelf)
                        AimInDirection(Vector3D.Zero, Vector3D.Normalize(ControlGravity));
                }
            }


            public void ThrustToVelocity(Vector3D velocity)
            {
                if (thrustPidController == null)
                    throw new Exception("wat de neuk thrustPid is null");
                if (thrustControl == null)
                    throw new Exception("wat de neuk thrustControl is null");
                if (ingameTime == null)
                    throw new Exception("wat de neuk ingameTime is null");

                Vector3D difference = velocity - ControlVelocity;
                double differenceMag = difference.Normalize();
                double percent = thrustPidController.NextValue(differenceMag, (lastTime - ingameTime.Time).TotalSeconds);
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

                if (double.IsNaN(stoppingDistance))
                    return 0;

                return stoppingDistance;
            }

            public void AimInDirection(Vector3D direction, Vector3D up, bool upDominant = false)
            {
                if (direction == Vector3D.Zero && up == Vector3D.Zero)
                {
                    gyroControl.StopRotation(gyros);
                }

                if (upDominant || direction == Vector3D.Zero)
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

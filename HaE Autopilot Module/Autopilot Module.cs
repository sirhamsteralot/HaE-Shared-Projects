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
            private PID_Controller PIDControl;
            private IngameTime ingameTime;

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

            public Autopilot_Module(GridTerminalSystemUtils GTS, IMyShipController controller, IngameTime ingameTime, PID_Controller.PIDSettings gyroPidSettings, PID_Controller.PIDSettings thrustPidSettings)
            {
                GTS.GridTerminalSystem.GetBlocksOfType(allThrusters);
                GTS.GridTerminalSystem.GetBlocksOfType(gyros);

                this.controller = controller;
                this.ingameTime = ingameTime;

                gyroControl = new AdvGyroControl(gyroPidSettings, ingameTime);
                thrustControl = new AdvThrustControl(controller, allThrusters, ingameTime, thrustPidSettings);
            }

            public void TravelToPosition(Vector3D position)
            {

            }

            public void ThrustToVelocity(Vector3D velocity)
            {
                thrustControl.ApplyForce(velocity * 100000);
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

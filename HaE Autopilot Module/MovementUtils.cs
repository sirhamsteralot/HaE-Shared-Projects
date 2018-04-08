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
        public static class MovementUtils
	    {
	        public static double CalculateStoppingDistance(ThrustersInDirection thrusters, IMyShipController controller)
            {
                Vector3D currentVelocityNormalized = controller.GetShipVelocities().LinearVelocity;
                double currentSpeed = currentVelocityNormalized.Normalize();

                double maxThrustInDirection = ThrustUtils.CalculateNewtonThrustInDirection(thrusters, currentVelocityNormalized);

                double deceleration = currentSpeed / controller.CalculateShipMass().PhysicalMass;
                double stoppingDistance = (currentSpeed * currentSpeed) / (2 * deceleration);

                return stoppingDistance;
            }
	    }
	}
}

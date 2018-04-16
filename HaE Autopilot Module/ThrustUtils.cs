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
        public class ThrustUtils
        {
            public static void SetThrust(List<IMyThrust> thrusters, Vector3D direction, double percent)
            {
                foreach (var thrust in thrusters)
                {
                    if (Vector3D.Dot(thrust.WorldMatrix.Backward, direction) > 0.99)
                    {
                        if (!thrust.Enabled)
                            thrust.Enabled = true;

                        thrust.ThrustOverridePercentage = (float)percent;
                    }
                }
            }

            public static void SetMinimumThrust(List<IMyThrust> thrusters, Vector3D direction, double percent)
            {
                foreach (var thrust in thrusters)
                {
                    if (Vector3D.Dot(thrust.WorldMatrix.Backward, direction) > 0.99)
                    {
                        if (!thrust.Enabled)
                            thrust.Enabled = true;

                        thrust.ThrustOverridePercentage = percent > thrust.ThrustOverridePercentage? (float)percent : thrust.ThrustOverridePercentage;
                    }
                }
            }

            public static void SetThrustBasedDot(List<IMyThrust> thrusters, Vector3D direction, double mulitplier = 1)
            {
                foreach (var thrust in thrusters)
                {
                    if (!thrust.Enabled)
                        thrust.Enabled = true;

                    double thrustpercentage = Vector3D.Dot(thrust.WorldMatrix.Backward, direction);
                    thrust.ThrustOverridePercentage = (float)(thrustpercentage * mulitplier);
                }
            }

            public static void SetThrustNotInDirection(List<IMyThrust> thrusters, Vector3D direction, double percent)
            {
                foreach (var thrust in thrusters)
                {
                    if (Vector3D.Dot(thrust.WorldMatrix.Backward, direction) < 0.9)
                    {
                        if (!thrust.Enabled)
                            thrust.Enabled = true;

                        thrust.ThrustOverridePercentage = (float)percent;
                    }
                }
            }

            public static Dictionary<Vector3D, List<IMyThrust>> SortThrustersByDirection(List<IMyThrust> thrusters, IMyShipController reference)
            {
                var thrustByDirection = new Dictionary<Vector3D, List<IMyThrust>>();

                foreach (var thruster in thrusters)
                {
                    var relativeThrustVector = Vector3D.TransformNormal(thruster.WorldMatrix.Backward, reference.WorldMatrix);

                    if (thrustByDirection[relativeThrustVector] == null)
                        thrustByDirection[relativeThrustVector] = new List<IMyThrust>();

                    thrustByDirection[relativeThrustVector].Add(thruster);
                }

                return thrustByDirection;
            }
        }
    }
}

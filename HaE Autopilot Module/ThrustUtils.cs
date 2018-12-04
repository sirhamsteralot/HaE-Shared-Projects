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
            public static double GetForwardThrust(List<IMyThrust>thrusters, IMyTerminalBlock reference)
            {
                double sum = 0;
                foreach (var thrust in thrusters)
                {
                    if (Vector3D.Dot(thrust.WorldMatrix.Backward, reference.WorldMatrix.Forward) > 0.999)
                    {
                        sum += thrust.MaxEffectiveThrust;
                    }
                }

                return sum;
            }

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

            public static void SetThrustPercentage(List<IMyThrust> thrusters, float percent)
            {
                foreach (var thruster in thrusters)
                {
                    if (!thruster.Enabled)
                        thruster.Enabled = true;

                    thruster.ThrustOverridePercentage = percent;
                }
            }

            public static void SetThrustPercentage(HashSet<IMyThrust> thrusters, float percent)
            {
                foreach (var thruster in thrusters)
                {
                    if (!thruster.Enabled)
                        thruster.Enabled = true;

                    thruster.ThrustOverridePercentage = percent;
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
        }
    }
}

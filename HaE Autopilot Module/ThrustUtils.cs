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

            /// <summary>
            /// Sets thrust in specified direction with great accuracy, this method is however more expensive.
            /// </summary>
            /// <param name="thrusters"> array of thrusters sorted by SortThrustByDirection</param>
            /// <param name="direction"> the normalized direction you want to fire the thrusters in</param>
            /// <param name="reference"> the reference, what everything is relative to</param>
            public static void PreciseThrustInDirection(ThrustersInDirection thrusters, Vector3D direction, IMyTerminalBlock reference)
            {
                Vector3D localDirection = Vector3D.TransformNormal(direction, MatrixD.Transpose(reference.WorldMatrix));

                double[] PercentPerDirection = new double[6];
                int minThrustersInDirection = thrusters.minThrustInDirection;

                foreach (var dir in Base6Directions.EnumDirections)
                {
                    List<IMyThrust> thrustersInDirection = thrusters[(int)dir];

                    if (thrustersInDirection == null)
                        continue;

                    Vector3D vectorDir = thrustersInDirection[0].WorldMatrix.Backward;

                    if (vectorDir.Dot(localDirection) < 0)
                        continue;

                    vectorDir = VectorUtils.Project(localDirection, vectorDir);

                    PercentPerDirection[(int)dir] = vectorDir.Length();
                }

                foreach (var dir in Base6Directions.EnumDirections)
                {
                    List<IMyThrust> thrustersInDirection = thrusters[(int)dir];

                    double directionMultiplier = PercentPerDirection[(int)dir] * (minThrustersInDirection / thrustersInDirection.Count);

                    if (directionMultiplier == 0)
                        continue;

                    foreach (IMyThrust thruster in thrustersInDirection)
                    {
                        thruster.ThrustOverridePercentage = (float)directionMultiplier;
                    }
                }
            }

            /// <summary>
            /// Returns array of lists keyed by Base6Directions.EnumDirections
            /// </summary>
            /// <param name="thrusters"> list of thrusters you want to sort</param>
            /// <param name="reference"> the reference, what everything is relative to</param>
            /// <returns> contains array of lists keyed by Base6Directions.EnumDirections</returns>
            public static ThrustersInDirection SortThrustByDirection(List<IMyThrust> thrusters, IMyShipController reference)
            {
                List<IMyThrust>[] sortedThrusters = new List<IMyThrust>[6];

                foreach (var thruster in thrusters)
                {
                    var thrustDir = Base6Directions.GetOppositeDirection(thruster.Orientation.Forward);
                    
                    foreach (var dir in Base6Directions.EnumDirections)
                    {
                        if (reference.Orientation.TransformDirection(dir) == thrustDir)
                        {
                            if (sortedThrusters[(int)dir] == null)
                                sortedThrusters[(int)dir] = new List<IMyThrust>();

                            sortedThrusters[(int)dir].Add(thruster);
                            break;
                        }
                    }
                }

                

                int minThrustersInDirection = 0;
                foreach (var thrustDirection in sortedThrusters)
                {
                    if (thrustDirection == null)
                        continue;

                    int amountInDirection = thrustDirection.Count;
                    minThrustersInDirection = (minThrustersInDirection > amountInDirection) ? amountInDirection : minThrustersInDirection;
                }

                return new ThrustersInDirection(sortedThrusters, minThrustersInDirection);
            }

            /// <summary>
            /// Returns the amount of thrust in a ceratain direction
            /// </summary>
            /// <param name="thrusters"></param>
            /// <param name="direction"></param>
            /// <returns></returns>
            public static double CalculateNewtonThrustInDirection(ThrustersInDirection thrusters, Vector3D direction)
            {
                double newtons = 0;

                foreach (List<IMyThrust> thrusterList in thrusters)
                {
                    foreach (var thruster in thrusterList)
                    {
                        Vector3D directionProjected = VectorUtils.Project(direction, thruster.WorldMatrix.Backward);
                        double percentOutput = directionProjected.Length() * (thrusters.minThrustInDirection / thrusterList.Count);

                        newtons += thruster.MaxEffectiveThrust * percentOutput;
                    }
                }

                return newtons;
            }
        }
    }
}

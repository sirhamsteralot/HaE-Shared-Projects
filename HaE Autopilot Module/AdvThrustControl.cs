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
        public class AdvThrustControl
	    {
            private IMyShipController controller;
            private IngameTime ingameTime;

            private Dictionary<Vector3D, List<IMyThrust>> sortedThrusters = new Dictionary<Vector3D, List<IMyThrust>>();

            private Dictionary<Vector3D, int> thrustersPerDirection = new Dictionary<Vector3D, int>();

            private Dictionary<Vector3D, ThrusterSide> thrustersInDirection = new Dictionary<Vector3D, ThrusterSide>();

            public AdvThrustControl(IMyShipController controller, List<IMyThrust> thrusters,IngameTime ingameTime)
            {
                this.controller = controller;
                this.ingameTime = ingameTime;

                SortThrustersByDirection(thrusters, controller);
                CountThrustersPerDirection();
            }

            public void CountThrustersPerDirection()
            {
                foreach (var thrustInDir in sortedThrusters.Keys)
                {
                    var thrusterList = sortedThrusters[thrustInDir];

                    thrustersPerDirection[thrustInDir] = thrusterList.Count;
                }
            }

            public void SortThrustersByDirection(List<IMyThrust> thrusters, IMyShipController reference)
            {
                var sortedThrusters = new Dictionary<Vector3D, List<IMyThrust>>();

                foreach (var thruster in thrusters)
                {
                    var relativeThrustVector = VectorUtils.TransformDirWorldToLocal(reference.WorldMatrix, thruster.WorldMatrix.Backward);

                    if (!sortedThrusters.ContainsKey(relativeThrustVector))
                        sortedThrusters[relativeThrustVector] = new List<IMyThrust>();

                    if (!thrustersInDirection.ContainsKey(relativeThrustVector))
                    {
                        ThrusterSide side = new ThrusterSide
                        {
                            thrustDirection = relativeThrustVector,
                            ingameTime = this.ingameTime,
                            thrusters = new HashSet<IMyThrust>()
                        };

                        thrustersInDirection[relativeThrustVector] = side;
                    }

                    thrustersInDirection[relativeThrustVector].thrusters.Add(thruster);
                    sortedThrusters[relativeThrustVector].Add(thruster);
                }
            }

            public void ApplyForce(Vector3D force, double percent)
            {
                Vector3D accel = force;
                double Magnitude = accel.Normalize();

                Vector3D localAccel = VectorUtils.TransformDirWorldToLocal(controller.WorldMatrix, accel);

                double scale = double.MaxValue;
                CalculateMag(localAccel, ref scale);

                scale *= percent;

                foreach (var thrustSide in thrustersInDirection.Values)
                {
                    thrustSide.ApplyForce(localAccel, scale);
                }
            }


            public void ApplyForce(Vector3D force)
            {
                Vector3D accel = force;
                double Magnitude = accel.Normalize();

                Vector3D localAccel = VectorUtils.TransformDirWorldToLocal(controller.WorldMatrix, accel);

                double scale = double.MaxValue;
                CalculateMag(localAccel, ref scale);

                foreach (var thrustSide in thrustersInDirection.Values)
                {
                    thrustSide.ApplyForce(localAccel, scale);
                }
            }

            public double CalculateMaxForce(Vector3D direction)
            {
                Vector3D localDir = VectorUtils.TransformDirWorldToLocal(controller.WorldMatrix, direction);

                double scale = double.MaxValue;
                CalculateMag(localDir, ref scale);

                Vector3D appliedForce = Vector3D.Zero;

                foreach (var thrustSide in thrustersInDirection.Values)
                {
                    appliedForce += thrustSide.thrustDirection * thrustSide.CalculateMaxForce(localDir, scale);
                }

                return appliedForce.Length();
            }

            private void CalculateMag(Vector3D localDir, ref double scale)
            {
                double mag;

                foreach (var dir in thrustersInDirection.Values)
                {
                    mag = Math.Max(0, localDir.Dot(dir.thrustDirection));
                    if (mag == 0)
                        continue;

                    var q = dir.MaxThrustInDirection / mag;
                    if (q < scale)
                        scale = q;
                }
            }


            private class ThrusterSide
            {
                public Vector3D thrustDirection;
                public double MaxThrustInDirection => CalculateEffectiveThrust();

                public IngameTime ingameTime;

                public int Amount => thrusters.Count;
                public HashSet<IMyThrust> thrusters;

                private float currentThrustAmount;

                private double cachedEffectiveThrustAmount;
                private TimeSpan lastCachingTime;
                public double cachingTimeOutS = 5;

                public void ApplyForce(Vector3D localDesiredAcceleration, double scale)
                {
                    var s = localDesiredAcceleration.Dot(thrustDirection) * scale / MaxThrustInDirection;
                    ApplyThrustPercentage(s);
                }

                public void NoForce()
                {
                    ApplyThrustPercentage(0);
                }

                public double CalculateMaxForce(Vector3D direction, double scale)
                {
                    double dotProd = direction.Dot(thrustDirection);
                    if (dotProd <= 0)
                        return 0;

                    var s = dotProd * scale;

                    return s;
                }

                private void ApplyThrustPercentage(double percentage)
                {
                    if (percentage > 0 && percentage != currentThrustAmount)
                    {
                        ThrustUtils.SetThrustPercentage(thrusters, (float)percentage);
                        currentThrustAmount = (float)percentage;
                    }
                    else if (percentage <= 0 && currentThrustAmount != 0)
                    {
                        ThrustUtils.SetThrustPercentage(thrusters, 0);
                        currentThrustAmount = 0;
                    }
                }

                private double CalculateEffectiveThrust()
                {
                    if ((ingameTime.Time - lastCachingTime).TotalSeconds > cachingTimeOutS || lastCachingTime == TimeSpan.Zero)
                    {
                        cachedEffectiveThrustAmount = 0;

                        foreach (var thruster in thrusters)
                        {
                            cachedEffectiveThrustAmount += thruster.MaxEffectiveThrust;
                        }

                        lastCachingTime = ingameTime.Time;
                    }


                    return cachedEffectiveThrustAmount;
                }
            }
        }
	}
}

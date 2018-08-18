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
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
	partial class Program
	{
        public class AdvancedSimTargeting
	    {
            public double tolerance;

            public Vector3D firingDirection;
            public bool continuous;
            public Action<Vector3D> onSimComplete;
            public Action onSimFail;

            private IMyShipController control;
            private MyDetectedEntityInfo target;

            private double speedlimit;
            private ProjectileInfo projectileInfo;
            private TargetInfo targetInfo;

            private long ticks;
            private double timescale;
            private double simulationClosest = double.MaxValue;
            private Vector3D closestProjPos;

            private Scheduler scheduler;

            public AdvancedSimTargeting(ProjectileInfo projectileInfo, MyDetectedEntityInfo target, IMyShipController control, double tolerance, bool continuous, double speedlimit = 100, double timescale = 1)
            {
                this.tolerance = tolerance;
                this.continuous = continuous;
                this.target = target;
                this.speedlimit = speedlimit;
                this.timescale = timescale;
                this.control = control;

                this.projectileInfo = projectileInfo;
                targetInfo = new TargetInfo(target, timescale);
            }

            public bool Tick()
            {
                if (scheduler == null)
                {
                    scheduler = new Scheduler(10);
                }
                if (scheduler.QueueCount == 0)
                {
                    scheduler.AddTask(new Task(RunSimulation()));
                }

                scheduler.Main();

                //P.Echo($"closest: {closest} tolerance: {tolerance}");

                if (simulationClosest < tolerance)
                {
                    return true;
                }

                return false;
            }

            public void UpdateRollingSimulation(Vector3D projectilPos, Vector3D projectileVel, MyDetectedEntityInfo freshEntityDetection)
            {
                projectileInfo.RollingSimulation(projectileVel, projectilPos);
                target = freshEntityDetection;
            }

            public IEnumerator<bool> RunSimulation()
            {
                if (!InitializeSimulation())
                    yield return false;

                // Simulation

                while (continuous)
                {
                    projectileInfo.LaunchProjectile(firingDirection * projectileInfo.projectileMaxSpeed);

                    do
                    {
                        projectileInfo.Tick();
                        targetInfo.Tick();

                    } while (ContinueSimulation());

                    P.Echo("sim finished!");

                    Vector3D difference = targetInfo.currentLocation - closestProjPos;
                    Vector3D missDir = difference;
                    double differenceMagnitude = missDir.Normalize();

                    double PFactor = MyMath.Clamp((float)differenceMagnitude, 0.001f, 1f);
                    Vector3D rejected = Vector3D.Reject(missDir, firingDirection) * PFactor * 0.001;

                    firingDirection += rejected;
                    firingDirection.Normalize();


                    projectileInfo.ResetSimulation();
                    targetInfo.RollingSimulation(target);
                    simulationClosest = double.MaxValue;

                    if (differenceMagnitude < tolerance)
                        onSimComplete?.Invoke(control.GetPosition() + firingDirection * 1000);
                    else
                        onSimFail?.Invoke();

                    yield return true;
                }
            }

            private bool InitializeSimulation()
            {
                if (projectileInfo == null)
                    return false;
                if (control == null)
                    return false;
                if (targetInfo == null)
                    return false;

                ticks = 0;
                firingDirection = Vector3D.Normalize(target.Position - projectileInfo.currentLocation);
                return true;
            }

            private bool ContinueSimulation()
            {
                double newClosest = Vector3D.DistanceSquared(projectileInfo.currentLocation, targetInfo.currentLocation);

                if (newClosest < simulationClosest && projectileInfo.currentTicks < projectileInfo.lifeTimeTicks)
                {
                    simulationClosest = newClosest;
                    closestProjPos = projectileInfo.currentLocation;
                    return true;
                }

                return false;
            }

            public class ProjectileInfo
            {
                public long lifeTimeTicks;
                public long currentTicks;

                public double timescale;
                public double projectileMaxSpeed;
                public Vector3D projectileAcceleration;
                public Vector3D currentLocation;
                public Vector3D currentVelocity;

                public Vector3D startLocation;
                public Vector3D startVelocity;

                public ProjectileInfo(long lifeTimeTicks, double projectileMaxSpeed, double timescale, Vector3D projectileAcceleration, Vector3D currentLocation, Vector3D currentVelocity)
                {
                    this.lifeTimeTicks = lifeTimeTicks;
                    this.timescale = timescale;
                    this.projectileMaxSpeed = projectileMaxSpeed;
                    this.projectileAcceleration = projectileAcceleration;
                    this.currentLocation = currentLocation;
                    this.currentVelocity = currentVelocity;
                }

                public void LaunchProjectile(Vector3D launchVelocity)
                {
                    currentVelocity = Vector3D.ClampToSphere((startVelocity * timescale) + (launchVelocity * timescale), projectileMaxSpeed * timescale);
                }

                public void Tick()
                {
                    currentVelocity = Vector3D.ClampToSphere(currentVelocity + projectileAcceleration * timescale, projectileMaxSpeed * timescale);
                    currentLocation = currentLocation += currentVelocity;

                    currentTicks++;
                }

                public void ResetSimulation()
                {
                    currentLocation = startLocation;
                    currentVelocity = startVelocity;
                    currentTicks = 0;
                }

                public void RollingSimulation(Vector3D startVel, Vector3D currentPos)
                {
                    startVelocity = startVel;
                    startLocation = currentPos;
                }
            }

            public class TargetInfo
            {
                public double timescale;
                public Vector3D currentLocation;
                public Vector3D currentVelocity;

                public TargetInfo(MyDetectedEntityInfo target, double timescale)
                {
                    this.timescale = timescale;
                    this.currentLocation = target.Position;
                    this.currentVelocity = target.Velocity;
                }

                public void RollingSimulation(MyDetectedEntityInfo target)
                {
                    this.currentLocation = target.Position;
                    this.currentVelocity = target.Velocity;
                }

                public void Tick()
                {
                    currentLocation = currentLocation += currentVelocity * timescale;
                }
            }
	    }
	}
}

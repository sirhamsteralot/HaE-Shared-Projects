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
        public class GridCannonTargeting
        {
            /*==========| Events |==========*/
            public Action<Vector3D> onRoutineFinish;
            public Action onRoutineFail;


            /*==========| blocks |==========*/
            private IMyShipController control;


            /*==========| Fields |==========*/
            private MyDetectedEntityInfo target;
            private double maxProjectileVel;
            private TimeSpan targetAquiredTime;
            private double targetTimeout;

            private bool keepRunning = true;
            

            public QuarticTargeting quartic;
            public AdvancedSimTargeting simTargeting;
            Scheduler internalScheduler;
            IngameTime ingameTime;

            public GridCannonTargeting(IMyShipController control, IngameTime ingameTime, double maxProjectileVel, double targetTimeout = 2.5)
            {
                this.targetTimeout = targetTimeout;
                this.control = control;
                this.maxProjectileVel = maxProjectileVel;
                this.keepRunning = true;
                this.ingameTime = ingameTime;

                internalScheduler = new Scheduler();
            }

            public void NewTarget(MyDetectedEntityInfo target)
            {
                targetAquiredTime = ingameTime.Time;
                keepRunning = true;

                if (this.target.EntityId == target.EntityId)
                {
                    UpdateTrackingInfo(target);

                    internalScheduler.AddTask(TargetingRoutine());
                    return;
                }
                
                this.target = target;
            }

            public void Tick()
            {
                internalScheduler.Main();

                if ((ingameTime.Time - targetAquiredTime).TotalSeconds > targetTimeout)
                    End();
            }

            public IEnumerator<bool> TargetingRoutine()
            {
                if (control.GetNaturalGravity() == Vector3D.Zero && control.GetShipSpeed() < 2)         // Easy
                {
                    quartic = new QuarticTargeting(control.GetVelocityVector(), control.GetPosition(), maxProjectileVel);

                    Vector3D? result = quartic.CalculateTrajectory(target);
                    if (result.HasValue)
                    {
                        onRoutineFinish?.Invoke(result.Value);
                    }
                    else
                    {
                        onRoutineFail?.Invoke();
                    }
                        
                    yield return false;
                }
                else                                                                                  // This may take a while...
                {
                    double timescale = 0.1;
                    var projectileInfo = new AdvancedSimTargeting.ProjectileInfo(480, maxProjectileVel, timescale, control.GetPosition(), control.GetVelocityVector());
                    simTargeting = new AdvancedSimTargeting(projectileInfo, target, control, ingameTime, 25, true, maxProjectileVel, timescale);
                    simTargeting.onSimComplete += OnSimValid;
                    simTargeting.onSimFail += OnSimFail;

                    yield return true;

                    while (keepRunning)
                    {
                        simTargeting.Tick();
                        yield return true;
                    }
                }
            }

            public void End()
            {
                keepRunning = false;
                OnSimFail();
            }

            public void UpdateTrackingInfo(MyDetectedEntityInfo newInfo)
            {
                if (simTargeting == null)
                    return;

                target = newInfo;
                simTargeting.UpdateRollingSimulation(control.GetPosition(), control.GetVelocityVector(), target);
            }

            private void OnSimValid(Vector3D closestPoint)
            {
                onRoutineFinish?.Invoke(closestPoint);
            }

            private void OnSimFail()
            {
                onRoutineFail?.Invoke();
            }
        }
    }
}

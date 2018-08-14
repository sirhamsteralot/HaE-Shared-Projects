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
            Action<Vector3D> onRoutineFinish;
            Action onRoutineFail;

            /*==========| blocks |==========*/
            public IMyShipController control;


            /*==========| Fields |==========*/
            private MyDetectedEntityInfo target;
            private double maxProjectileVel;

            private bool keepRunning = true;
            

            QuarticTargeting quartic;
            AdvancedSimTargeting simTargeting;
            Scheduler internalScheduler;

            public GridCannonTargeting(IMyShipController control, double maxProjectileVel, bool keepRunning)
            {
                this.control = control;
                this.maxProjectileVel = maxProjectileVel;
                this.keepRunning = keepRunning;

                internalScheduler = new Scheduler();
            }

            public void NewTarget(MyDetectedEntityInfo target)
            {
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
            }

            public IEnumerator<bool> TargetingRoutine()
            {
                if (control.GetNaturalGravity() == Vector3D.Zero && control.GetShipSpeed() < 2)         // Easy
                {
                    quartic = new QuarticTargeting(control.GetVelocityVector(), control.GetPosition(), maxProjectileVel);

                    Vector3D? result = quartic.CalculateTrajectory(target);
                    if (result.HasValue)
                        onRoutineFinish?.Invoke(result.Value);
                    else
                        onRoutineFail?.Invoke();
                    yield return false;
                }
                else                                                                                  // This may take a while...
                {
                    var projectileInfo = new AdvancedSimTargeting.ProjectileInfo(3600, maxProjectileVel, 1, Vector3D.Zero, control.GetPosition(), control.GetVelocityVector());
                    simTargeting = new AdvancedSimTargeting(projectileInfo, target, control, 10, true, maxProjectileVel);
                    simTargeting.onSimComplete += OnSimValid;
                    simTargeting.onSimFail += OnSimFail;

                    yield return true;

                    while (keepRunning)
                    {
                        simTargeting.RunSimulation();
                        yield return true;
                    }
                }
            }

            public void End()
            {
                keepRunning = false;
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

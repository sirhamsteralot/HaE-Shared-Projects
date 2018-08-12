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
            public IMyCubeBlock topBlock;
            public IMyShipController control;


            /*==========| Fields |==========*/
            public Vector3D targetingDirection;
            public MyDetectedEntityInfo target;
            public double maxProjectileVel;

            public bool keepRunning = true;
            

            QuarticTargeting quartic;
            AdvancedSimTargeting simTargeting;

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
                    var projectileInfo = new AdvancedSimTargeting.ProjectileInfo(3600, maxProjectileVel, 1, Vector3D.Zero, topBlock.GetPosition(), control.GetVelocityVector());
                    simTargeting = new AdvancedSimTargeting(projectileInfo, target, control, 10, true, maxProjectileVel);
                    simTargeting.onSimComplete += OnSimComplete;
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
                simTargeting.UpdateRollingSimulation(topBlock.GetPosition(), control.GetVelocityVector(), target);
            }

            public Vector3D GetTargetingDirection()
            {
                return targetingDirection;
            }

            private void OnSimComplete(Vector3D dir)
            {
                targetingDirection = dir;
            }

            private void OnSimFail()
            {
                targetingDirection = Vector3D.Zero;
            }
        }
    }
}

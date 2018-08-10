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


            /*==========| Fields |==========*/
            public Vector3D targetingDirection;
            public MyDetectedEntityInfo target;
            public double maxProjectileVel;

            public IMyShipController control;

            QuarticTargeting quartic;
            Simulated_Targeting simTargeting;

            public IEnumerator<bool> TargetingRoutine()
            {
                if (control.GetNaturalGravity() == Vector3D.Zero && control.GetShipSpeed() < 2)         // Easy
                {
                    quartic = new QuarticTargeting(control.GetShipVelocities().LinearVelocity, control.GetPosition(), maxProjectileVel);

                    Vector3D? result = quartic.CalculateTrajectory(target);
                    if (result.HasValue)
                        onRoutineFinish?.Invoke(result.Value);
                    else
                        onRoutineFail?.Invoke();
                    yield return false;
                } else                                                                                  // This may take a while...
                {
                    // TODO: Simtargeting
                }
            }

        }
	}
}

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
        public class Simulated_Targeting
	    {
            /*=========| Hardcoded  data |=========*/
            public double timeLimit = 3600;
            public double tolerance = 100;
            public double negationFactor = 0.01;

            /*=========| Neccesairy data |=========*/
            private IMyShipController reference;
            private Vector3D targetPosition;

            private double projectileSpeed;
            private double projectileSpeedCap;
            private double projectileAcceleration;
            private Vector3D projectileForward;
            private Vector3D projectileStartPosition;

            private MyDetectedEntityInfo planet;
            private double surfaceGravity;


            /*======| Global Operating Data |======*/
            private IEnumerator<bool> Coroutine;
            private int ticks = 0;

            private double planetRadius;
            private double GravityCutoffHeight;

            private BoundingSphereD gravitySphere;
            private BoundingSphereD planetSphere;


            /*===========| Output Data |===========*/
            bool succes = false;


            public Simulated_Targeting(IMyShipController reference, Vector3D targetPosition, Vector3D projectileStartPosition, 
                                    Vector3D projectileForward, double projectileAcceleration,
                                    MyDetectedEntityInfo planet, double surfaceGravity, double projectileSpeed = 100,
                                    double projectileSpeedCap = 104.75, double planetRadiusOverride = 0)
            {
                this.reference = reference;
                this.targetPosition = targetPosition;
                this.projectileSpeed = projectileSpeed;
                this.projectileSpeedCap = projectileSpeedCap;
                this.projectileStartPosition = projectileStartPosition;
                this.planet = planet;
                this.surfaceGravity = surfaceGravity;
                this.projectileForward = projectileForward;
                this.projectileAcceleration = projectileAcceleration;

                if (planetRadiusOverride != 0)
                    planetRadius = planetRadiusOverride;
                else
                    planetRadius = planet.BoundingBox.Size.X / 1.12 / 2;


                GravityCutoffHeight = (Math.Pow(surfaceGravity, 1.0 / 7.0) * (planetRadius * 1.12)) / Math.Pow(0.3924, 1.0 / 7.0);

                gravitySphere = new BoundingSphereD(planet.Position, GravityCutoffHeight);
                planetSphere = new BoundingSphereD(planet.Position, (targetPosition - planet.Position).Length());

                Coroutine = TargetingRoutine();
            }

            public Simulated_Targeting( IMyShipController reference, Vector3D targetPosition, Vector3D projectileStartPosition, 
                                        Vector3D projectileForward, double projectileAcceleration, double projectileSpeed = 100,
                                        double projectileSpeedCap = 104.75)
            {
                this.reference = reference;
                this.targetPosition = targetPosition;
                this.projectileSpeed = projectileSpeed;
                this.projectileSpeedCap = projectileSpeedCap;
                this.projectileStartPosition = projectileStartPosition;
                this.projectileForward = projectileForward;
                this.projectileAcceleration = projectileAcceleration;

                Coroutine = TargetingRoutine();
            }


            public bool Calculate()
            {

                if (Coroutine != null)
                    return !Coroutine.MoveNext();
                else
                    return false;
            }

            /*=======================================| Private Code, calculations etc. |=======================================*/

            /*========| Operating Data |========*/
            Vector3D CurrentFiringDirection;

            Vector3D CurrentProjectileLocation;
            Vector3D CurrentProjectileVelocity;

            double currentSimulationTime;
            double originalMissMagnitude;


            /*==========| Functions |===========*/

            IEnumerator<bool> TargetingRoutine()
            {
                InitTargetingRoutine();
                yield return true;

                while (!SimulateTrajectory())
                {
                    Vector3D missDirection = (targetPosition - CurrentProjectileLocation);
                    double missMagnitude = missDirection.LengthSquared();
                    missDirection.Normalize();

                    if (ticks == 0)
                        originalMissMagnitude = missMagnitude;

                    double PFactor = MyMath.Clamp((float)missMagnitude / (float)originalMissMagnitude, 0.001f, 1f);

                    Vector3D rejected = Vector3D.Reject(missDirection, CurrentFiringDirection) * PFactor * negationFactor;

                    CurrentFiringDirection += rejected;
                    CurrentFiringDirection.Normalize();

                    ticks++;
                    yield return true;
                }
            }

            void InitTargetingRoutine()
            {
                ticks = 0;
                CurrentFiringDirection = Vector3D.Normalize(targetPosition - reference.GetPosition());
            }

            bool SimulateTrajectory()
            {
                //Init Simulation environment
                CurrentProjectileLocation = projectileStartPosition;
                CurrentProjectileVelocity = CurrentFiringDirection * projectileSpeed;
                currentSimulationTime = 0;

                do
                {
                    CurrentProjectileLocation += CurrentProjectileVelocity;

                    Vector3D acceleration = (Vector3D.Normalize(planet.Position - CurrentProjectileLocation) * GetGravityAtAltitude(Vector3D.Distance(planet.Position, CurrentProjectileLocation))) + (projectileForward * projectileAcceleration);
                    CurrentProjectileVelocity = AccelVelocityClamped(CurrentProjectileVelocity, acceleration);
                } while (planetSphere.Contains(CurrentProjectileLocation) == ContainmentType.Disjoint && currentSimulationTime++ <= timeLimit);

                if (Vector3D.DistanceSquared(CurrentProjectileLocation, targetPosition) <= tolerance * tolerance)
                    return true;

                return false;
            }

            /*=======| Helper Functions |========*/

            double GetGravityAtAltitude(double RadiusFromCentre)
            {
                if (RadiusFromCentre < planetRadius * 1.12)
                    return surfaceGravity;

                double gravity = surfaceGravity * Math.Pow((planetRadius * 1.12) / RadiusFromCentre, 7);
                if (gravity >= 0.05)
                    return gravity;
                else
                    return 0;
            }

            Vector3D AccelVelocityClamped(Vector3D Velocity, Vector3D acceleration)
            {
                return Vector3D.ClampToSphere((Velocity + acceleration), projectileSpeedCap);
            }


            /*====| Public Getter/Setters |=====*/
            public Vector3D GetCurrentFiringDirection()
            {
                return CurrentFiringDirection;
            }

            public Vector3D GetCurrentTarget()
            {
                return targetPosition;
            }
        }
	}
}

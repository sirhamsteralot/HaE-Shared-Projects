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
        public class ProjectorVisualization
	    {
            IMyProjector projector;
            Vector3I projectionOffset;

            public ProjectorVisualization(IMyProjector projector, Vector3I projectionOffset)
            {
                this.projector = projector;
                this.projectionOffset = projectionOffset;
            }

            public void UpdatePosition(Vector3D locationInWorld)
            {
                if (projector == null)
                    return;

                Vector3D locationInGrid = Vector3D.Transform(locationInWorld, MatrixD.Invert(projector.WorldMatrix)) / projector.CubeGrid.GridSize;
                locationInGrid += (Vector3D)projectionOffset;
                Vector3I locationRounded = new Vector3I(
                                                            (int)-Math.Round(locationInGrid.X),
                                                            (int)-Math.Round(locationInGrid.Y),
                                                            (int)-Math.Round(locationInGrid.Z)
                                                        );

                projector.ProjectionOffset = locationRounded;
                projector.UpdateOffsetAndRotation();
            }

            public Vector3D GetPosition()
            {
                return projector.GetPosition();
            }

            public void Enable()
            {
                projector.Enabled = true;
            }

            public void Disable()
            {
                projector.Enabled = false;
            }
        }
	}
}

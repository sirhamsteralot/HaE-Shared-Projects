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
    partial class Program : MyGridProgram
    {
        public const string REFERENCENAME = "Remote Control";
        public const string TARGETCAMNAME = "Targeting Camera";
        public const string TEXTOUTNAME = "TextOut";

        public const double PAINTINGDISTANCE = 1000;
        public const int TicksToRunProfiler = 100;

        Profiler profiler = new Profiler(TicksToRunProfiler, true);
        GridTerminalSystemUtils gridTerminalSystemUtils;
        IMyShipController reference;
        IMyCameraBlock targetingCamera;
        IMyTextPanel textOut;

        EntityTracking_Module entityTracking;
        Autopilot_Module autopilotModule;



        public void SubConstructor()
        {
            //UpdateType
            Runtime.UpdateFrequency = UpdateFrequency.Update1 | UpdateFrequency.Update100;

            //Generic Inits
            gridTerminalSystemUtils = new GridTerminalSystemUtils(Me, GridTerminalSystem);
            reference = GridTerminalSystem.GetBlockWithName(REFERENCENAME) as IMyShipController;
            targetingCamera = GridTerminalSystem.GetBlockWithName(TARGETCAMNAME) as IMyCameraBlock;
            textOut = GridTerminalSystem.GetBlockWithName(TEXTOUTNAME) as IMyTextPanel;

            //Module Inits
            entityTracking = new EntityTracking_Module(gridTerminalSystemUtils, reference, targetingCamera);
            autopilotModule = new Autopilot_Module(gridTerminalSystemUtils, reference);
        }

        public void SubMain(string argument, UpdateType updateSource)
        {
            EntityTrackingTests(argument, updateSource);
            AutopilotTests(argument, updateSource);
        }

        bool started = false;
        public void AutopilotTests(string argument, UpdateType updateSource)
        {
            switch (argument)
            {
                case "Toggle":
                    started = !started;
                    break;
            }

            if (started)
            {
                Vector3D directionToTravel = Vector3D.Normalize(Vector3D.Zero - reference.GetPosition());
                Vector3D directionAlign = -Vector3D.Normalize(reference.GetNaturalGravity());

                autopilotModule.PointInDirection(directionToTravel, directionAlign);
            }
        }

        public void EntityTrackingTests(string argument, UpdateType updateSource)
        {
            entityTracking.Poll();

            switch (argument)
            {
                case "Paint":
                    entityTracking.PaintTarget(PAINTINGDISTANCE);
                    break;
            }

            if ((updateSource & UpdateType.Update100) != 0)
            {
                entityTracking.TimeoutEntities(TimeSpan.FromSeconds(5));
            }

            textOut.WritePublicText(entityTracking.known_Objects.ToString());
        }

        //MAIN/CONSTR
        public Program()
        {
            profiler.OnProfilingFinished += OnProfilingFinished;
            DebugUtils.ConstructorWrapper(SubConstructor, this);
        }

        public void OnProfilingFinished()
        {
            profiler.DumpValues(Me);
            Echo("Profiling finished");
            profiler = null;
        }

        public void Main(string argument, UpdateType updateSource)
        {
            profiler?.AddValue(Runtime.LastRunTimeMs);
            DebugUtils.MainWrapper(SubMain, argument, updateSource, this);
        }
    }
}
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
        public const string MonoLCDName = "LCD";

        public const double PAINTINGDISTANCE = 1000;
        public const int TicksToRunProfiler = 100;

        public static Program P;

        IngameTime ingameTime;
        Profiler profiler = new Profiler(TicksToRunProfiler, false);
        GridTerminalSystemUtils gridTerminalSystemUtils;
        IMyShipController reference;
        IMyCameraBlock targetingCamera;
        IMyTextPanel textOut;
        IMyTextPanel monoOut;

        Random random = new Random();

        EntityTracking_Module entityTracking;
        Autopilot_Module autopilotModule;
        LCDDrawingLib drawingLib;
        Scheduler scheduler;

        PID_Controller.PIDSettings gyroPidSettings = new PID_Controller.PIDSettings
        {
            PGain = 1,
            DerivativeGain = 0,
            IntegralGain = 0
        };

        PID_Controller.PIDSettings thrustPidSettings = new PID_Controller.PIDSettings
        {
            PGain = 1,
            DerivativeGain = 0,
            IntegralGain = 0
        };

        public void SubConstructor()
        {
            //DEBUG ATTACHMENTS
            P = this;

            //Timing
            Runtime.UpdateFrequency = UpdateFrequency.Update1 | UpdateFrequency.Update100;
            ingameTime = new IngameTime();

            //Generic Inits
            gridTerminalSystemUtils = new GridTerminalSystemUtils(Me, GridTerminalSystem);
            reference = GridTerminalSystem.GetBlockWithName(REFERENCENAME) as IMyShipController;
            targetingCamera = GridTerminalSystem.GetBlockWithName(TARGETCAMNAME) as IMyCameraBlock;
            textOut = GridTerminalSystem.GetBlockWithName(TEXTOUTNAME) as IMyTextPanel;
            monoOut = GridTerminalSystem.GetBlockWithName(MonoLCDName) as IMyTextPanel;

            //Module Inits
            //entityTracking = new EntityTracking_Module(gridTerminalSystemUtils, reference, targetingCamera);
            //autopilotModule = new Autopilot_Module(gridTerminalSystemUtils, reference, ingameTime, gyroPidSettings, thrustPidSettings, entityTracking);
            scheduler = new Scheduler();
            drawingLib = new LCDDrawingLib(87,87, Color.Green);


        }

        public void SubMain(string argument, UpdateType updateSource)
        {

            //Scheduler run
            scheduler.Main();

            //EntityTrackingTests(argument, updateSource);
            //AutopilotTests(argument, updateSource);
            LCDLibTests(argument, updateSource);
        }

        public void LCDLibTests(string argument, UpdateType updateSource)
        {
            switch (argument)
            {
                case "DrawCircle":
                    int ranX = random.Next(0, 86);
                    int ranY = random.Next(0, 86);
                    Vector2I pos = new Vector2I(ranX, ranY);

                    var circle = new Circle(pos, random.Next(1, 10), Color.White, false);

                    drawingLib.AddElement(circle);
                    Task generator = new Task(drawingLib.Generate(), LCD_OnGenerationComplete);
                    scheduler.AddTask(generator);
                    break;
            }
        }

        public void LCD_OnGenerationComplete()
        {
            monoOut.WritePublicText(drawingLib.Draw());
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
                Vector3D position = Vector3D.Zero;
                Vector3D direction = position - reference.GetPosition();
                direction.Normalize();

                autopilotModule.TravelToPosition(position, true);
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

            monoOut.WritePublicText(entityTracking.known_Objects.ToString());
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
            ingameTime.Tick(Runtime.TimeSinceLastRun.TotalMilliseconds);

            profiler?.AddValue(Runtime.LastRunTimeMs);
            DebugUtils.MainWrapper(SubMain, argument, updateSource, this);
        }
    }
}
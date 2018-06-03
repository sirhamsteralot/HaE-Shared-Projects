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

        //EntityTracking_Module entityTracking;
        //Autopilot_Module autopilotModule;
        MonospaceDrawingLib drawingLib;
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
            drawingLib = new MonospaceDrawingLib(175,175, Color.Black);
            drawingLib.onRenderDone = OnRenderDone;
            drawingLib.AddRenderTask(drawingLib.ReGenerate(), OnRenderDone);

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
                    int ranX = random.Next(0, 174);
                    int ranY = random.Next(0, 174);
                    Vector2I pos = new Vector2I(ranX, ranY);

                    var circle = new FillCircle(pos, random.Next(1, 50), Color.White);

                    drawingLib.AddElement(circle);
                    break;

                case "DrawLine":
                    int ranX0 = random.Next(0, 174);
                    int ranY0 = random.Next(0, 174);

                    int ranX1 = random.Next(0, 174);
                    int ranY1 = random.Next(0, 174);

                    Vector2I pointOne = new Vector2I(ranX0, ranY0);
                    Vector2I pointTwo = new Vector2I(ranX1, ranY1);

                    var line = new Line(pointOne, pointTwo, Color.White);
                    drawingLib.AddElement(line);
                    break;
                case "DrawEllipse":
                    int ranEX0 = random.Next(0, 174);
                    int ranEY0 = random.Next(0, 174);

                    int ranEX1 = random.Next(0, 174);
                    int ranEY1 = random.Next(0, 174);

                    Vector2I pointEOne = new Vector2I(ranEX0, ranEY0);
                    Vector2I pointETwo = new Vector2I(ranEX1, ranEY1);

                    var elipse = new FillEllipse(pointEOne, pointETwo, Color.White);
                    drawingLib.AddElement(elipse);
                    break;
                case "DrawRect":
                    int ranRX0 = random.Next(0, 174);
                    int ranRY0 = random.Next(0, 174);

                    int ranRX1 = random.Next(0, 174);
                    int ranRY1 = random.Next(0, 174);

                    Vector2I pointROne = new Vector2I(ranRX0, ranRY0);
                    Vector2I pointRTwo = new Vector2I(ranRX1, ranRY1);

                    var rectangle = new FillRectangle(pointROne, pointRTwo, Color.White);
                    drawingLib.AddElement(rectangle);
                    break;

                case "DrawPolygon":
                    var positions = new List<Vector2I>();

                    for (int i = 0; i < random.Next(3, 16); i++)
                    {
                        positions.Add(VectorUtils.GenerateRandomVector2I(random));
                    }

                    var polygon = new FillPolygon(positions, Color.White);
                    drawingLib.AddElement(polygon);
                    break;

                case "DrawCharacter":
                    int ranCX0 = random.Next(0, 174);
                    int ranCY0 = random.Next(0, 174);
                    Vector2I posChar = new Vector2I(ranCX0, ranCY0);

                    var character = new ResizableCharacter(posChar, 'E', Color.White, 1);
                    drawingLib.AddElement(character);
                    break;

                case "HelloWorld":
                    int ranTX0 = random.Next(0, 174);
                    int ranTY0 = random.Next(0, 174);
                    Vector2I posT = new Vector2I(ranTX0, ranTY0);

                    var text = new Text(posT, "HELLO_WORLD?", Color.White, 1);
                    drawingLib.AddElement(text);
                    break;
            }

            drawingLib.RunRenderer();
        }

        public void OnRenderDone()
        {
            Echo("Rendering...");
            Echo($"Left in Queue: {drawingLib.RenderQueueSize}");
            monoOut.WritePublicText(drawingLib.Draw());
        }

        bool started = false;
        //public void AutopilotTests(string argument, UpdateType updateSource)
        //{
        //    switch (argument)
        //    {
        //        case "Toggle":
        //            started = !started;
        //            break;
        //    }

        //    if (started)
        //    {
        //        Vector3D position = Vector3D.Zero;
        //        Vector3D direction = position - reference.GetPosition();
        //        direction.Normalize();

        //        autopilotModule.TravelToPosition(position, true);
        //    }
        //}

        //public void EntityTrackingTests(string argument, UpdateType updateSource)
        //{
        //    entityTracking.Poll();

        //    switch (argument)
        //    {
        //        case "Paint":
        //            entityTracking.PaintTarget(PAINTINGDISTANCE);
        //            break;
        //    }

        //    if ((updateSource & UpdateType.Update100) != 0)
        //    {
        //        entityTracking.TimeoutEntities(TimeSpan.FromSeconds(5));
        //    }

        //    monoOut.WritePublicText(entityTracking.known_Objects.ToString());
        //}

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
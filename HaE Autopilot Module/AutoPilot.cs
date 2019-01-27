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
        public class AutoPilot
        {
            public AutopilotMode currentMode;

            private Vector3D _targetLocation;
            private Vector3D _targetDirection;
            private Vector3D _targetDirUp;
            private Vector3D _targetVelocity;
            private bool _enableAvoidance;
            private bool _upDominant;
            private double _maxVelocity;
            private double _safetyMarginMult;

            public Autopilot_Module autopilotModule;

            public AutoPilot(GridTerminalSystemUtils GTS, IMyShipController controller, IngameTime time,
                PID_Controller.PIDSettings gyroPidSettings, PID_Controller.PIDSettings thrustPidSettings, 
                EntityTracking_Module entityTracking)
            {
                autopilotModule = new Autopilot_Module(GTS, controller, time, gyroPidSettings, thrustPidSettings,  entityTracking);
            }

            public AutoPilot(GridTerminalSystemUtils GTS, IMyShipController controller, IngameTime time, EntityTracking_Module entityTracking)
            {
                autopilotModule = new Autopilot_Module(GTS, controller, time, entityTracking);
            }

            public void TravelToPosition(Vector3D position, bool enableAvoidance, double maximumVelocity = 100, double safetyMargin = 1.25)
            {
                _targetLocation = position;
                _enableAvoidance = enableAvoidance;
                _maxVelocity = maximumVelocity;
                _safetyMarginMult = safetyMargin;

                currentMode |= AutopilotMode.TravelToPosition;
            }

            public void ThrustToVelocity(Vector3D velocity)
            {
                _targetVelocity = velocity;

                currentMode |= AutopilotMode.ThrustToVelocity;
            }

            public void AimInDirection(Vector3D direction, Vector3D up, bool upDominant = false)
            {
                _targetDirection = direction;
                _targetDirUp = up;
                _upDominant = upDominant;
            }

            public void Main()
            {
                if ((currentMode & AutopilotMode.ThrustToVelocity) != 0)
                {
                    autopilotModule.ThrustToVelocity(_targetVelocity);
                }
                else if ((currentMode & AutopilotMode.TravelToPosition) != 0)
                {
                    autopilotModule.TravelToPosition(_targetLocation, _maxVelocity, _safetyMarginMult, (currentMode & AutopilotMode.AimInDirection) != 0);
                }

                if ((currentMode & AutopilotMode.AimInDirection) != 0)
                {
                    autopilotModule.AimInDirection(_targetDirection, _targetDirUp);
                }
            }

            [Flags]
            public enum AutopilotMode
            {
                TravelToPosition,
                ThrustToVelocity,
                AimInDirection
            }
        }
    }
}

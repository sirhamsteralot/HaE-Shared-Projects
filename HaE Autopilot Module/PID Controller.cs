using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript
{
    public class PID_Controller
    {
        private double _proportionalGain;
        private double _integralGain;
        private double _derivativeGain;

        private double _pastError;
        private double _currentError;

        private double _integral;
        private double _iterationTimeS;

        public PID_Controller(double proportionalGain, double integralGain, double derivativeGain)
        {
            _proportionalGain = proportionalGain;
            _integralGain = integralGain;
            _derivativeGain = derivativeGain;
        }

        public PID_Controller(PIDSettings settings)
        {
            _proportionalGain = settings.PGain;
            _integralGain = settings.IntegralGain;
            _derivativeGain = settings.DerivativeGain;
        }

        public double NextValue(double error, double iterationTimeS)
        {
            _pastError = _currentError;
            _currentError = error;
            _iterationTimeS = iterationTimeS;

            return Proportional() + Integral() + Derivative();
        }

        private double Proportional()
        {
            return _proportionalGain * _currentError;
        }

        private double Integral()
        {
            _integral += _currentError * _iterationTimeS;

            return _integralGain * _integral;
        }

        private double Derivative()
        {
            double deltaError = _currentError - _pastError;

            return _derivativeGain * (deltaError * _iterationTimeS);
        }

        public struct PIDSettings
        {
            public double PGain;
            public double IntegralGain;
            public double DerivativeGain;
        };
    }
}

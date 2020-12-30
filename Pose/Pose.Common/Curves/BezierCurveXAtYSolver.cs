using System;

namespace Pose.Common.Curves
{
    /// <summary>
    /// Improves performance of multiple calculations on a single <see cref="BezierCurve"/> by reusing intermediate data. If you only need to calculate once for a curve, use <see cref="BezierMath"/> instead.
    /// </summary>
    public class BezierCurveSolver
    {
        private readonly float _tolerance;
        private readonly Polynomial3 _fx;
        private readonly Polynomial3 _fy;

        public BezierCurveSolver(BezierCurve bezierCurve, float tolerance = 0.002f)
        {
            _tolerance = tolerance;
            _fx = bezierCurve.GetFx();
            _fy = bezierCurve.GetFy();
        }

        public float SolveYAtX(float x)
        {
            // see BezierMath for explanation of what and why this does.

            if (x < 0f || x > 1f)
                throw new ArgumentOutOfRangeException(nameof(x));

            var fx = new Polynomial3(_fx.A, _fx.B, _fx.C, _fx.D - x); // Newton-Raphson finds 0-point of function (t where f(t)=0), but we want t where f(t) = x, so subtract x from D of the polynomial to be able to look for 0-point.
            var fxDerivative = fx.GetDerivative();

            // use Newton-Raphson to find a T that yields an fx(t) closer to 0 than allowed tolerance.

            var s = x; // initial guess = what t would be if the spline was perfectly linear.
            for (var i = 0; i < 100; i++) // lets not try more than 100 times :)
            {
                var resultX = fx.Solve(s);
                if (resultX >= 0 && resultX < _tolerance || resultX < 0 && resultX > -_tolerance)
                    break;
                s -= resultX / fxDerivative.Calc(s);
            }

            return _fy.Solve(s);
        }
    }
}

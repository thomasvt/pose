using System;
using Pose.Common.Curves;

namespace Pose.Domain.Animations
{
    public readonly struct InterpolationData
    {
        private readonly BezierCurveSolver _bezierCurveSolver;

        public InterpolationData(CurveType type, BezierCurve? bezierCurve = null)
        {
            if (type == CurveType.Bezier && bezierCurve == null)
                throw new Exception("Must pass a beziercurve when interpolation type is Bezier");

            Type = type;
            BezierCurve = bezierCurve;
            _bezierCurveSolver = bezierCurve.HasValue ? new BezierCurveSolver(bezierCurve.Value) : null;
        }

        public CurveType Type { get; }

        /// <summary>
        /// Only used when Type is Bezier.
        /// </summary>
        public BezierCurve? BezierCurve { get; }

        /// <summary>
        /// Calculates Y value (percentage) on the curve at value X (percentage).
        /// </summary>
        public float CalculateY(in float x)
        {
            switch (Type)
            {
                case CurveType.Linear:
                    return x;
                case CurveType.Bezier:
                    return _bezierCurveSolver.SolveYAtX(x);
                case CurveType.Hold:
                    return 0;
                default:
                    throw new NotSupportedException($"InterpolationData has an unsupported type \"{Type}\".");
            }
        }

        public static InterpolationData Hold => new InterpolationData(CurveType.Hold);
        public static InterpolationData Linear => new InterpolationData(CurveType.Linear);
        public static InterpolationData EasingLow => GetEasingInterpolation(1f / 3);
        public static InterpolationData EasingMedium => GetEasingInterpolation(0.5f);
        public static InterpolationData EasingHigh => GetEasingInterpolation(1f);

        public static InterpolationData GetEasingInterpolation(float easingPercentage)
        {
            if (easingPercentage < 0f || easingPercentage > 1f)
                throw new ArgumentOutOfRangeException(nameof(easingPercentage));
            return new InterpolationData(CurveType.Bezier, Common.Curves.BezierCurve.GetEasingCurve(easingPercentage));
        }
    }
}

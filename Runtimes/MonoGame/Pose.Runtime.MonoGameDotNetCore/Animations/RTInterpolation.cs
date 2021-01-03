using Pose.Common.Curves;

namespace Pose.Runtime.MonoGameDotNetCore.Animations
{
    internal readonly struct RTInterpolation
    {
        public readonly BezierCurveSolver BezierCurveSolver;
        public readonly float LeftKeyTime;
        public readonly float Duration;
        public readonly float LeftKeyValue;
        public readonly float RightKeyValue;
        public readonly CurveType CurveType;

        public RTInterpolation(float leftTime, float leftValue, float rightTime, float rightValue, CurveType curveType, BezierCurve? bezierCurve = null, float bezierTolerance = 0.002f)
        {
            LeftKeyTime = leftTime;
            Duration = rightTime - leftTime;
            LeftKeyValue = leftValue;
            RightKeyValue = rightValue;
            CurveType = curveType;
            BezierCurveSolver = bezierCurve.HasValue ? new BezierCurveSolver(bezierCurve.Value, bezierTolerance) : null;
        }
    }
}

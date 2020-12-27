using Pose.Domain.Curves;

namespace Pose.Runtime.MonoGameDotNetCore.Animations
{
    /// <summary>
    /// A segment between 2 Keys of a single PropertyAnimation.
    /// </summary>
    internal class RTSegment
    {
        public readonly RTKey BeginKey;
        public readonly RTKey EndKey;
        public readonly float Duration;
        public readonly BezierCurveSolver BezierCurveSolver;
        public readonly CurveType CurveType;

        public RTSegment(RTKey beginKey, RTKey endKey, CurveType curveType, BezierCurve? bezierCurve = null)
        {
            BeginKey = beginKey;
            EndKey = endKey;
            Duration = endKey.Time - beginKey.Time;
            CurveType = curveType;
            BezierCurveSolver = bezierCurve.HasValue ? new BezierCurveSolver(bezierCurve.Value) : null;
        }
    }
}

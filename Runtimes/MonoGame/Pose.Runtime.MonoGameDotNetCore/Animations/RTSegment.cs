using Pose.Common.Curves;

namespace Pose.Runtime.MonoGameDotNetCore.Animations
{
    /// <summary>
    /// A segment between 2 Keys of a single PropertyAnimation. 
    /// </summary>
    internal class RTSegment
    {
        public readonly BezierCurveSolver BezierCurveSolver;
        public readonly float BeginTime;
        public readonly float EndTime;
        public readonly float LeftKeyTime;
        public readonly float Duration;
        public readonly float LeftKeyValue;
        public readonly float RightKeyValue;
        public readonly CurveType CurveType;
        

        public RTSegment(float beginTime, float endTime, float leftKeyTime, float duration, float leftKeyValue, float rightKeyValue, CurveType curveType, BezierCurve? bezierCurve = null, float bezierTolerance = 0.002f)
        {
            BeginTime = beginTime;
            EndTime = endTime;
            LeftKeyTime = leftKeyTime;
            Duration = duration;
            LeftKeyValue = leftKeyValue;
            RightKeyValue = rightKeyValue;
            CurveType = curveType;
            BezierCurveSolver = bezierCurve.HasValue ? new BezierCurveSolver(bezierCurve.Value, bezierTolerance) : null;
        }


        
    }
}

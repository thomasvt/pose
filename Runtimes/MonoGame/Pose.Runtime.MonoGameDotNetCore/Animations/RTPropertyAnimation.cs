using System;
using Pose.Domain.Curves;

namespace Pose.Runtime.MonoGameDotNetCore.Animations
{
    internal class RTPropertyAnimation
    {
        public readonly int NodeIdx;
        public readonly NodeProperty NodeProperty;
        private readonly RTSegment[] _segments;
        private int _currentSegmentIdx;

        public RTPropertyAnimation(int nodeIdx, NodeProperty nodeProperty, RTSegment[] segments)
        {
            if (segments == null || segments.Length == 0)
                throw new ArgumentException("The segments array must not be null or empty.");

            NodeIdx = nodeIdx;
            NodeProperty = nodeProperty;
            _segments = segments;
            _currentSegmentIdx = 0;
        }

        /// <summary>
        /// Sets the first <see cref="RTSegment"/> as current segment.
        /// </summary>
        internal void Reset()
        {
            _currentSegmentIdx = 0;
        }

        /// <summary>
        /// Plays forward to the given time and returns the interpolated property value for that point in time.
        /// </summary>
        internal float PlayForwardTo(in float time)
        {
            ref var currentSegment = ref _segments[_currentSegmentIdx];
            // move to next segment until 'time' is inside segment. Also loops to first segment if last was reached and t is still outside the segment (because t will loop back to 0 by caller when isLoop = true).
            while (time >= currentSegment.EndKey.Time || time < currentSegment.BeginKey.Time)
            {
                _currentSegmentIdx = (_currentSegmentIdx + 1) % _segments.Length;
                currentSegment = ref _segments[_currentSegmentIdx];
            }

            // calc where we are on this segment in [0,1] percent
            var t = (time - currentSegment.BeginKey.Time) / currentSegment.Duration;
            // get interpolation value according to interpolationtype
            var y = 0f;
            if (currentSegment.CurveType == CurveType.Bezier)
            {
                y = currentSegment.BezierCurveSolver.SolveYAtX(t);
            }
            else if (currentSegment.CurveType == CurveType.Linear)
            {
                y = t;
            }

            return currentSegment.BeginKey.Value * (1 - y) + currentSegment.EndKey.Value * y;
        }
    }
}

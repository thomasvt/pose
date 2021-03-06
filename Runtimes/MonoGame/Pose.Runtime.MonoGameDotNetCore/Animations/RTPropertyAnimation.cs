﻿using System;
using Pose.Common.Curves;

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
            // move to next segment until 'time' is inside current segment. Also wrap around to first segment if last is passed.
            while (time >= currentSegment.EndTime || time < currentSegment.BeginTime)
            {
                _currentSegmentIdx = (_currentSegmentIdx + 1) % _segments.Length;
                currentSegment = ref _segments[_currentSegmentIdx];
            }

            var interpolation = currentSegment.Interpolation;
            // calc where we are on this segment in [0,1] percent
            var t = (time - interpolation.LeftKeyTime) / interpolation.Duration;
            // get interpolation value according to interpolationtype
            var y = 0f;
            if (interpolation.CurveType == CurveType.Bezier)
            {
                y = interpolation.BezierCurveSolver.SolveYAtX(t);
            }
            else if (interpolation.CurveType == CurveType.Linear)
            {
                y = t;
            }

            return interpolation.LeftKeyValue * (1f - y) + interpolation.RightKeyValue * y;
        }
    }
}

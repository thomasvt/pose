using System;

namespace Pose.SceneEditor.MouseOperations
{
    /// <summary>
    /// Tracks a user's gesturing input of an angle. It enables inputting obtuse angles and even multiple full rotations.
    /// </summary>
    internal class RotationTracker
    {
        private const float A360 = MathF.PI * 2f;
        private const float A180 = MathF.PI;

        private float _previousInputAngle;

        public RotationTracker(float initialInputAngle, float startValue)
        {
            Angle = startValue;
            _previousInputAngle = initialInputAngle;
        }

        /// <param name="angle">Should be between -pi and +pi</param>
        public void AddAngleInput(float angle)
        {
            var delta = GetNormalizedAngleDifference(angle, _previousInputAngle);
            Angle += delta;
            _previousInputAngle = angle;
        }

        private static float GetNormalizedAngleDifference(float a, float b)
        {
            var delta = a - b;
            if (delta < -A180)
                return -A360 - delta;
            if (delta > A180)
                return A360 - delta;
            return delta;
        }

        public float Angle { get; private set; }
    }
}

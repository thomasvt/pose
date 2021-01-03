namespace Pose.Runtime.MonoGameDotNetCore.Animations
{
    /// <summary>
    /// A segment between 2 Keys of a single PropertyAnimation. 
    /// </summary>
    internal class RTSegment
    {
        public readonly float BeginTime;
        public readonly float EndTime;
        public readonly RTInterpolation Interpolation;

        public RTSegment(float beginTime, float endTime, RTInterpolation interpolation)
        {
            BeginTime = beginTime;
            EndTime = endTime;
            Interpolation = interpolation;
        }
    }
}

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

        public RTSegment(RTKey beginKey, RTKey endKey)
        {
            BeginKey = beginKey;
            EndKey = endKey;
            Duration = endKey.Time - beginKey.Time;
        }
    }
}

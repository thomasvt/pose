namespace Pose.Domain.Editor.Messages
{
    public class CurrentAnimationChanged
    {
        public ulong AnimationId { get; }

        public CurrentAnimationChanged(ulong animationId)
        {
            AnimationId = animationId;
        }
    }
}

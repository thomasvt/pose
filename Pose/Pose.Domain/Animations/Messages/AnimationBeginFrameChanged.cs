namespace Pose.Domain.Animations.Messages
{
    public class AnimationBeginFrameChanged
    {
        public ulong AnimationId { get; }
        public readonly int BeginFrame;

        internal AnimationBeginFrameChanged(ulong animationId, int beginFrame)
        {
            AnimationId = animationId;
            BeginFrame = beginFrame;
        }
    }
}

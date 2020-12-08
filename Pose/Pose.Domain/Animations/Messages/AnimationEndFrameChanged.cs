namespace Pose.Domain.Animations.Messages
{
    public class AnimationEndFrameChanged
    {
        public ulong AnimationId { get; }
        public int EndFrame { get; }

        internal AnimationEndFrameChanged(ulong animationId, int endFrame)
        {
            AnimationId = animationId;
            EndFrame = endFrame;
        }
    }
}

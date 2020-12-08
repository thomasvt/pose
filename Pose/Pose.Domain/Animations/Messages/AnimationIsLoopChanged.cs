namespace Pose.Domain.Animations.Messages
{
    public class AnimationIsLoopChanged
    {
        public ulong AnimationId { get; }
        public bool IsLoop { get; }

        public AnimationIsLoopChanged(ulong animationId, bool isLoop)
        {
            AnimationId = animationId;
            IsLoop = isLoop;
        }
    }
}

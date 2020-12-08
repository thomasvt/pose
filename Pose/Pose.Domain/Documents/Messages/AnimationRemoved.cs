namespace Pose.Domain.Documents.Messages
{
    public class AnimationRemoved
    {
        public ulong AnimationId { get; }

        public AnimationRemoved(ulong animationId)
        {
            AnimationId = animationId;
        }
    }
}

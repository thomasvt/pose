namespace Pose.Domain.Documents.Messages
{
    /// <summary>
    /// Published just before removing an animation.
    /// </summary>
    public class AnimationRemoving
    {
        public ulong AnimationId { get; }

        public AnimationRemoving(ulong animationId)
        {
            AnimationId = animationId;
        }
    }
}

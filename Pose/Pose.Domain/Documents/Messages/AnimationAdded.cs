namespace Pose.Domain.Documents.Messages
{
    public class AnimationAdded
    {
        public ulong AnimationId { get; }
        public string Name { get; }

        public AnimationAdded(ulong animationId, string name)
        {
            AnimationId = animationId;
            Name = name;
        }
    }
}

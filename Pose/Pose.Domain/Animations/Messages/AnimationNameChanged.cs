namespace Pose.Domain.Animations.Messages
{
    public class AnimationNameChanged
    {
        public ulong AnimationId { get; }
        public string Name { get; }

        public AnimationNameChanged(ulong animationId, string name)
        {
            AnimationId = animationId;
            Name = name;
        }
    }
}

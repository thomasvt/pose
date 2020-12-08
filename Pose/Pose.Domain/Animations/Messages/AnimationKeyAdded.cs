namespace Pose.Domain.Animations.Messages
{
    public class AnimationKeyAdded
    {
        public ulong PropertyAnimationId { get; }
        public ulong KeyId { get; }
        public int Frame { get; }

        public AnimationKeyAdded(ulong propertyAnimationId, ulong keyId, int frame)
        {
            PropertyAnimationId = propertyAnimationId;
            KeyId = keyId;
            Frame = frame;
        }
    }
}

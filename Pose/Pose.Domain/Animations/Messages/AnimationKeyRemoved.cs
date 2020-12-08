namespace Pose.Domain.Animations.Messages
{
    public class AnimationKeyRemoved
    {
        public ulong PropertyAnimationId { get; }
        public ulong KeyId { get; }
        public int Frame { get; }

        public AnimationKeyRemoved(ulong propertyAnimationId, ulong keyId, int frame)
        {
            PropertyAnimationId = propertyAnimationId;
            KeyId = keyId;
            Frame = frame;
        }
    }
}

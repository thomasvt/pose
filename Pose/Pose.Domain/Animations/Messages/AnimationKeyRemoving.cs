namespace Pose.Domain.Animations.Messages
{
    public class AnimationKeyRemoving
    {
        public ulong KeyId { get; }

        public AnimationKeyRemoving(ulong keyId)
        {
            KeyId = keyId;
        }
    }
}

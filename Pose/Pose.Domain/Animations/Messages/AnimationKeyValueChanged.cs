namespace Pose.Domain.Animations.Messages
{
    public class AnimationKeyValueChanged
    {
        public ulong KeyId { get; }
        public float Value { get; }

        public AnimationKeyValueChanged(ulong keyId, float value)
        {
            KeyId = keyId;
            Value = value;
        }
    }
}

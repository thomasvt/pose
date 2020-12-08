namespace Pose.Domain.Animations.Messages
{
    public class AnimationKeyInterpolationDataChanged
    {
        public ulong KeyId { get; }
        public InterpolationData Data { get; }

        public AnimationKeyInterpolationDataChanged(ulong keyId, InterpolationData data)
        {
            KeyId = keyId;
            Data = data;
        }
    }
}

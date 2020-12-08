using Pose.Domain.Documents;

namespace Pose.Domain.Animations.Events
{
    internal class AnimationKeyAddedEvent
    : IEvent
    {
        public ulong PropertyAnimationId { get; }
        public ulong KeyId { get; }
        public int Frame { get; }
        public float Value { get; }
        public InterpolationData Interpolation { get; }

        public AnimationKeyAddedEvent(ulong propertyAnimationId, ulong keyId, int frame, float value, InterpolationData interpolation)
        {
            PropertyAnimationId = propertyAnimationId;
            KeyId = keyId;
            Frame = frame;
            Value = value;
            Interpolation = interpolation;
        }

        public void PlayForward(IEditableDocument document)
        {
            document.AddKey(KeyId, PropertyAnimationId, Frame, Value, Interpolation);
        }

        public void PlayBackward(IEditableDocument document)
        {
            document.RemoveKey(KeyId);
        }
    }
}

using Pose.Domain.Documents;

namespace Pose.Domain.Animations.Events
{
    internal class AnimationKeyRemovedEvent
    : IEvent
    {
        public ulong PropertyAnimationId { get; }
        public ulong KeyId { get; }
        public int Frame { get; }
        public float UndoValue { get; }
        public InterpolationData UndoInterpolation { get; }

        public AnimationKeyRemovedEvent(ulong propertyAnimationId, ulong keyId, int frame, float undoValue, InterpolationData undoInterpolation)
        {
            PropertyAnimationId = propertyAnimationId;
            KeyId = keyId;
            Frame = frame;
            UndoValue = undoValue;
            UndoInterpolation = undoInterpolation;
        }

        public void PlayForward(IEditableDocument document)
        {
            document.RemoveKey(KeyId);
        }

        public void PlayBackward(IEditableDocument document)
        {
            document.AddKey(KeyId, PropertyAnimationId, Frame, UndoValue, UndoInterpolation);            
        }
    }
}

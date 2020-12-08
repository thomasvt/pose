using Pose.Domain.Documents;

namespace Pose.Domain.Animations.Events
{
    internal class AnimationBeginFrameChangedEvent
    : IEvent
    {
        public ulong AnimationId { get; }
        public int UndoValue { get; }
        public int NewValue { get; }

        public AnimationBeginFrameChangedEvent(ulong animationId, int undoValue, int newValue)
        {
            AnimationId = animationId;
            UndoValue = undoValue;
            NewValue = newValue;
        }

        public void PlayForward(IEditableDocument document)
        {
            var animation = document.GetAnimation(AnimationId) as IEditableAnimation;
            animation.ChangeBeginFrame(NewValue);
        }

        public void PlayBackward(IEditableDocument document)
        {
            var animation = document.GetAnimation(AnimationId) as IEditableAnimation;
            animation.ChangeBeginFrame(UndoValue);
        }
    }
}

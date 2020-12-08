using Pose.Domain.Documents;

namespace Pose.Domain.Animations.Events
{
    internal class AnimationEndFrameChangedEvent
    : IEvent
    {
        public ulong AnimationId { get; }
        public int UndoValue { get; }
        public int NewValue { get; }

        public AnimationEndFrameChangedEvent(ulong animationId, int undoValue, int newValue)
        {
            AnimationId = animationId;
            UndoValue = undoValue;
            NewValue = newValue;
        }

        public void PlayForward(IEditableDocument document)
        {
            var animation = document.GetAnimation(AnimationId) as IEditableAnimation;
            animation.ChangeEndFrame(NewValue);
        }

        public void PlayBackward(IEditableDocument document)
        {
            var animation = document.GetAnimation(AnimationId) as IEditableAnimation;
            animation.ChangeEndFrame(UndoValue);
        }
    }
}

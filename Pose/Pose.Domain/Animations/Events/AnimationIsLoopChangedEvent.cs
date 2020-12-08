using System;
using Pose.Domain.Documents;

namespace Pose.Domain.Animations.Events
{
    internal class AnimationIsLoopChangedEvent
    : IEvent
    {
        public ulong AnimationId { get; }
        public bool UndoIsLoop { get; }
        public bool IsLoop { get; }

        public AnimationIsLoopChangedEvent(ulong animationId, bool undoIsLoop, bool isLoop)
        {
            AnimationId = animationId;
            UndoIsLoop = undoIsLoop;
            IsLoop = isLoop;
        }

        public void PlayForward(IEditableDocument document)
        {
            var animation = document.GetAnimation(AnimationId) as IEditableAnimation;
            animation.SetIsLoop(IsLoop);
        }

        public void PlayBackward(IEditableDocument document)
        {
            var animation = document.GetAnimation(AnimationId) as IEditableAnimation;
            animation.SetIsLoop(UndoIsLoop);
        }
    }
}

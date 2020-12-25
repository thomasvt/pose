using System;
using Pose.Domain.Documents;

namespace Pose.Domain.Animations.Events
{
    internal class AnimationRenamedEvent
    : IEvent
    {
        public ulong AnimationId { get; }
        public string UndoName { get; }
        public string Name { get; }

        public AnimationRenamedEvent(ulong animationId, string undoName, string name)
        {
            AnimationId = animationId;
            UndoName = undoName;
            Name = name;
        }

        public void PlayForward(IEditableDocument document)
        {
            var animation = document.GetAnimation(AnimationId) as IEditableAnimation;
            animation.ChangeName(Name);
        }

        public void PlayBackward(IEditableDocument document)
        {
            var animation = document.GetAnimation(AnimationId) as IEditableAnimation;
            animation.ChangeName(UndoName);
        }
    }
}

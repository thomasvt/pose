using Pose.Domain.Documents;

namespace Pose.Domain.Animations.Events
{
    internal class NodeAnimationCollectionAddedEvent
    : IEvent
    {
        public ulong AnimationId { get; }
        public ulong NodeId { get; }

        public NodeAnimationCollectionAddedEvent(ulong animationId, ulong nodeId)
        {
            AnimationId = animationId;
            NodeId = nodeId;
        }

        public void PlayForward(IEditableDocument document)
        {
            document.AddNodeAnimationCollection(AnimationId, NodeId);
        }

        public void PlayBackward(IEditableDocument document)
        {
            document.RemoveNodeAnimationCollection(AnimationId, NodeId);
        }
    }
}

using Pose.Domain.Documents;
using Pose.Domain.Nodes;
using Pose.Domain.Nodes.Properties;

namespace Pose.Domain.Animations.Events
{
    internal class PropertyAnimationAddedEvent
    : IEvent
    {
        public ulong PropertyAnimationId { get; }
        public ulong AnimationId { get; }
        public ulong NodeId { get; }
        public PropertyType Property { get; }

        public PropertyAnimationAddedEvent(ulong propertyAnimationId, ulong animationId, ulong nodeId, PropertyType property)
        {
            PropertyAnimationId = propertyAnimationId;
            AnimationId = animationId;
            NodeId = nodeId;
            Property = property;
        }

        public void PlayForward(IEditableDocument document)
        {
            document.AddPropertyAnimation(PropertyAnimationId, AnimationId, NodeId, Property);
        }

        public void PlayBackward(IEditableDocument document)
        {
            document.RemovePropertyAnimation(PropertyAnimationId);
        }
    }
}

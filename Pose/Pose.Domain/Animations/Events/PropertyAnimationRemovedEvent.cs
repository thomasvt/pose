using Pose.Domain.Documents;
using Pose.Domain.Nodes;
using Pose.Domain.Nodes.Properties;

namespace Pose.Domain.Animations.Events
{
    internal class PropertyAnimationRemovedEvent
    : IEvent
    {
        public ulong AnimationId { get; }
        public ulong PropertyAnimationId { get; }
        public ulong NodeId { get; }
        public PropertyType Property { get; }

        public PropertyAnimationRemovedEvent(ulong animationId, ulong propertyAnimationId, ulong nodeId, PropertyType property)
        {
            AnimationId = animationId;
            PropertyAnimationId = propertyAnimationId;
            NodeId = nodeId;
            Property = property;
        }

        public void PlayForward(IEditableDocument document)
        {
            document.RemovePropertyAnimation(PropertyAnimationId);
        }

        public void PlayBackward(IEditableDocument document)
        {
            document.AddPropertyAnimation(PropertyAnimationId, AnimationId, NodeId, Property);
        }
    }
}

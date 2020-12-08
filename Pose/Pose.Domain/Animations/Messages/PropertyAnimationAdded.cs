using Pose.Domain.Nodes;
using Pose.Domain.Nodes.Properties;

namespace Pose.Domain.Animations.Messages
{
    public class PropertyAnimationAdded
    {
        public ulong PropertyAnimationId { get; }
        public ulong AnimationId { get; }
        public ulong NodeId { get; }
        public PropertyType Property { get; }

        public PropertyAnimationAdded(ulong propertyAnimationId, ulong animationId, ulong nodeId, PropertyType property)
        {
            PropertyAnimationId = propertyAnimationId;
            AnimationId = animationId;
            NodeId = nodeId;
            Property = property;
        }
    }
}

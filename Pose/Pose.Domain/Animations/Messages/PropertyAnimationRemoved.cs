using Pose.Domain.Nodes;
using Pose.Domain.Nodes.Properties;

namespace Pose.Domain.Animations.Messages
{
    /// <summary>
    /// Raised when an entire propertyanimation row (and thus also the keys) is removed.
    /// </summary>
    public class PropertyAnimationRemoved
    {
        public ulong PropertyAnimationId { get; }
        public ulong AnimationId { get; }
        public ulong NodeId { get; }
        public PropertyType Property { get; }

        public PropertyAnimationRemoved(ulong propertyAnimationId, ulong animationId, ulong nodeId, PropertyType property)
        {
            PropertyAnimationId = propertyAnimationId;
            AnimationId = animationId;
            NodeId = nodeId;
            Property = property;
        }
    }
}

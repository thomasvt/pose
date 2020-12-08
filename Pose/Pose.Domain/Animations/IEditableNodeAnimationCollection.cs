using Pose.Domain.Nodes;
using Pose.Domain.Nodes.Properties;

namespace Pose.Domain.Animations
{
    internal interface IEditableNodeAnimationCollection
    {
        void AddPropertyAnimation(PropertyAnimation propertyAnimation);
        void RemovePropertyAnimation(ulong propertyAnimationId, PropertyType property);
    }
}
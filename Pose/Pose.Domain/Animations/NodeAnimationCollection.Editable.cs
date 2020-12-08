using Pose.Domain.Animations.Messages;
using Pose.Domain.Nodes.Properties;

namespace Pose.Domain.Animations
{
    /// <summary>
    /// The atomic modifications that can be performed. These should only be called by Events from History.
    /// </summary>
    public partial class NodeAnimationCollection
    {
        void IEditableNodeAnimationCollection.AddPropertyAnimation(PropertyAnimation propertyAnimation)
        {
            PropertyAnimations.Add(propertyAnimation.Property, propertyAnimation);
            _messageBus.Publish(new PropertyAnimationAdded(propertyAnimation.Id, Animation.Id, Node.Id, propertyAnimation.Property));
        }

        void IEditableNodeAnimationCollection.RemovePropertyAnimation(ulong propertyAnimationId, PropertyType property)
        {
            PropertyAnimations.Remove(property);
            _messageBus.Publish(new PropertyAnimationRemoved(propertyAnimationId, Animation.Id, Node.Id, property));
        }
    }
}

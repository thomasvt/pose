using Pose.Domain.Animations.Messages;

namespace Pose.Domain.Animations
{
    /// <summary>
    /// The atomic modifications that can be performed. These should only be called by Events from History.
    /// </summary>
    public partial class PropertyAnimation
    {
        void IEditablePropertyAnimation.AddKey(Key key)
        {
            _keys.Add(key.Frame, key);
            MessageBus.Publish(new AnimationKeyAdded(Id, key.Id, key.Frame));
        }

        void IEditablePropertyAnimation.RemoveKey(Key key)
        {
            MessageBus.Publish(new AnimationKeyRemoving(key.Id));
            _keys.Remove(key.Frame);
            MessageBus.Publish(new AnimationKeyRemoved(Id, key.Id, key.Frame));
        }
    }
}

using Pose.Domain.Animations.Messages;

namespace Pose.Domain.Animations
{
    public partial class Key
    {
        void IEditableKey.ChangeValue(float value)
        {
            Value = value;
            MessageBus.Publish(new AnimationKeyValueChanged(Id, value));
        }

        void IEditableKey.ChangeInterpolationData(InterpolationData data)
        {
            Interpolation = data;
            MessageBus.Publish(new AnimationKeyInterpolationDataChanged(Id, data));
        }
    }
}
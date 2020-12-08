using Pose.Domain.Animations.Events;
using Pose.Framework.Messaging;

namespace Pose.Domain.Animations
{
    public partial class Key
    : Entity, IEditableKey
    {
        public ulong PropertyAnimationId { get; }
        public int Frame { get; }
        public float Value { get; private set; }
        public InterpolationData Interpolation { get; internal set; }
        
        public Key(IMessageBus messageBus, ulong id, ulong propertyAnimationId, int frame, float value, InterpolationData interpolation)
        : base(messageBus, id)
        {
            PropertyAnimationId = propertyAnimationId;
            Frame = frame;
            Value = value;
            Interpolation = interpolation;
        }

        public void ChangeValue(IUnitOfWork uow, in float value)
        {
            if (Value == value)
                return;
            uow.Execute(new KeyValueChangedEvent(Id, Value, value));
        }

        public void ChangeInterpolation(IUnitOfWork uow, InterpolationData interpolationData)
        {
            uow.Execute(new KeyInterpolationDataChangedEvent(Id, Interpolation, interpolationData));
        }

        public override string ToString()
        {
            return $"[K:{Frame}]";
        }
    }
}

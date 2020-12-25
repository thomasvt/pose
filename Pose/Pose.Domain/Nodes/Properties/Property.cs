using System;
using Pose.Domain.Nodes.Messages;
using Pose.Domain.Nodes.Properties.Events;
using Pose.Framework.Messaging;

namespace Pose.Domain.Nodes.Properties
{
    public partial class Property : IEditableProperty
    {
        private readonly IMessageBus _messageBus;
        private readonly Node _node;
        private readonly PropertyType _propertyType;
        private readonly bool _isIncrementalValueProperty; // the current animation value = design + increment OR just the increment ?

        public Property(IMessageBus messageBus, Node node, PropertyType propertyType)
        {
            _messageBus = messageBus;
            _node = node;
            _propertyType = propertyType;
            _isIncrementalValueProperty = IsIncrementalValueProperty(propertyType);
        }

        /// <summary>
        /// The value of this property in Design mode, aka Rest pose.
        /// </summary>
        public float DesignValue { get; private set; }

        /// <summary>
        /// The value added to the BaseValue to get the current animation value of this property.
        /// </summary>
        public float AnimateIncrement { get; private set; }

        public float DesignVisualValue { get; private set; }
        public float AnimateVisualValue { get; private set; }

        /// <summary>
        /// Sets the animation value of the property.
        /// </summary>
        internal void SetAnimateValue(IUnitOfWork uow, float value)
        {
            var increment = _isIncrementalValueProperty ? value - DesignValue : value;

            if (AnimateIncrement == increment)
                return;

            uow.Execute(new PropertyAnimateIncrementChangedEvent(_node.Id, _propertyType, AnimateIncrement, increment));
        }

        /// <summary>
        /// Returns the final animation value for this property, given a certain animationincrement value. The result depends on the fact that this is an incrementalvalue vs absolutevalue property.
        /// Incremental means the animationincrement is added to the design value, Absolute just uses the animationIncrement by itself.
        /// </summary>
        public float GetAnimateNetValue(float animationIncrement)
        {
            return _isIncrementalValueProperty ? DesignValue + animationIncrement : animationIncrement;
        }

        public static bool ValueToBool(float value)
        {
            return value != 0;
        }

        /// <summary>
        /// Checks if the property's value uses the system where animation values are the sum of designvalue + animationincrement.
        /// </summary>
        private static bool IsIncrementalValueProperty(PropertyType propertyType)
        {
            return propertyType != PropertyType.Visibility;
        }

        /// <summary>
        /// Sets the BaseValue of this property. 
        /// </summary>
        internal void SetDesignValue(IUnitOfWork uow, float value)
        {
            if (DesignValue == value)
                return;

            uow.Execute(new PropertyDesignValueChangedEvent(_node.Id, _propertyType, DesignValue, value));
        }

        public PropertyValueSet GetPropertyValueSet()
        {
            return new PropertyValueSet(_propertyType, DesignValue, AnimateIncrement);
        }

        public void SetDesignVisualValue(in float value)
        {
            DesignVisualValue = value;
            ValueChanged?.Invoke();
            _messageBus.Publish(new NodePropertyValueChanged(_node.Id, _propertyType, _node.IsBulkUpdate));
        }

        public void SetAnimateVisualValue(in float value)
        {
            AnimateVisualValue = value;
            ValueChanged?.Invoke();
            _messageBus.Publish(new NodePropertyValueChanged(_node.Id, _propertyType, _node.IsBulkUpdate));
        }

        public void ResetDesignVisualValue()
        {
            DesignVisualValue = DesignValue;
        }

        public void ResetAnimateVisualValue()
        {
            AnimateVisualValue = GetAnimateNetValue(AnimateIncrement);
        }

        internal void SetDesignValueInternal(in float value)
        {
            DesignValue = value;
            DesignVisualValue = value;
        }

        public static readonly float TrueValue = 1f;
        public static readonly float FalseValue = 0f;

        public event Action ValueChanged;
    }
}

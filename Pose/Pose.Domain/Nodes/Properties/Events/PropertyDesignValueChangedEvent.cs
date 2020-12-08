using Pose.Domain.Documents;

namespace Pose.Domain.Nodes.Properties.Events
{
    internal class PropertyDesignValueChangedEvent
    : IEvent
    {
        public ulong NodeId { get; }
        public PropertyType PropertyType { get; }
        public float UndoValue { get; }
        public float Value { get; }

        public PropertyDesignValueChangedEvent(ulong nodeId, PropertyType propertyType, float undoValue, float value)
        {
            NodeId = nodeId;
            PropertyType = propertyType;
            UndoValue = undoValue;
            Value = value;
        }

        public void PlayForward(IEditableDocument document)
        {
            var node = document.GetNode(NodeId);
            var property = node.GetProperty(PropertyType) as IEditableProperty;
            property.SetDesignValue(Value);
        }

        public void PlayBackward(IEditableDocument document)
        {
            var node = document.GetNode(NodeId);
            var property = node.GetProperty(PropertyType) as IEditableProperty;
            property.SetDesignValue(UndoValue);
        }
    }
}

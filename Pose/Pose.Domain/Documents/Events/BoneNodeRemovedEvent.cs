using System.Collections.Generic;
using Pose.Domain.Nodes.Properties;

namespace Pose.Domain.Documents.Events
{
    internal class BoneNodeRemovedEvent
    : IEvent
    {
        public ulong NodeId { get; }
        public string Name { get; }
        public ulong? ParentNodeId { get; }
        public int Index { get; }
        public List<PropertyValueSet> PropertyValues { get; }

        public BoneNodeRemovedEvent(in ulong nodeId, string name, in ulong? parentNodeId, in int index, List<PropertyValueSet> propertyValues)
        {
            NodeId = nodeId;
            Name = name;
            ParentNodeId = parentNodeId;
            Index = index;
            PropertyValues = propertyValues;
        }

        public void PlayForward(IEditableDocument document)
        {
            document.RemoveNode(NodeId);
        }

        public void PlayBackward(IEditableDocument document)
        {
            document.AddBoneNode(NodeId, Name, ParentNodeId, Index, PropertyValues);
        }
    }
}

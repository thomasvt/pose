using System.Collections.Generic;
using Pose.Domain.Animations;
using Pose.Domain.Nodes;
using Pose.Domain.Nodes.Properties;

namespace Pose.Domain.Documents.Events
{
    internal class SpriteNodeRemovedEvent
    : IEvent
    {
        public ulong NodeId { get; }
        public string Name { get; }
        public SpriteReference SpriteRef { get; }
        public ulong? ParentNodeId { get; }
        public int Index { get; }
        public List<PropertyValueSet> PropertyValues { get; }

        public SpriteNodeRemovedEvent(ulong nodeId, string name, SpriteReference spriteRef, ulong? parentNodeId, int index, List<PropertyValueSet> propertyValues)
        {
            NodeId = nodeId;
            Name = name;
            SpriteRef = spriteRef;
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
            document.AddSpriteNode(NodeId, Name, SpriteRef, ParentNodeId, Index, PropertyValues);
        }
    }
}

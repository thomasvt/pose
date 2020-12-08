namespace Pose.Domain.Documents.Events
{
    internal class SpriteNodeAddedEvent
    : IEvent
    {
        public ulong NodeId { get; }
        public string Name { get; }
        public SpriteReference SpriteRef { get; }
        public ulong? ParentNodeId { get; }
        public int Index { get; }

        public SpriteNodeAddedEvent(ulong nodeId, string name, SpriteReference spriteRef, ulong? parentNodeId, int index) 
        {
            NodeId = nodeId;
            Name = name;
            SpriteRef = spriteRef;
            ParentNodeId = parentNodeId;
            Index = index;
        }

        public void PlayForward(IEditableDocument document)
        {
            document.AddSpriteNode(NodeId, Name, SpriteRef, ParentNodeId, Index, null);
        }

        public void PlayBackward(IEditableDocument document)
        {
            document.RemoveNode(NodeId);
        }
    }
}

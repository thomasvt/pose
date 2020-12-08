namespace Pose.Domain.Documents.Events
{
    internal class BoneNodeAddedEvent
    : IEvent
    {
        public ulong NodeId { get; }
        public string Name { get; }
        public ulong? ParentNodeId { get; }
        public int Index { get; }

        public BoneNodeAddedEvent(ulong nodeId, string name, ulong? parentNodeId, int index) 
        {
            NodeId = nodeId;
            Name = name;
            ParentNodeId = parentNodeId;
            Index = index;
        }

        public void PlayForward(IEditableDocument document)
        {
            document.AddBoneNode(NodeId, Name, ParentNodeId, Index, null);
        }

        public void PlayBackward(IEditableDocument document)
        {
            document.RemoveNode(NodeId);
        }
    }
}

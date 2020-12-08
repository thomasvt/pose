namespace Pose.Domain.Documents.Messages
{
    public class NodeAttachedToParent
    {
        public ulong NodeId { get; }
        public ulong? SourceParentNodeId { get; }
        public ulong? TargetParentNodeId { get; }
        public int Index { get; }

        public NodeAttachedToParent(ulong nodeId, ulong? sourceParentNodeId, ulong? targetParentNodeId, int index)
        {
            NodeId = nodeId;
            SourceParentNodeId = sourceParentNodeId;
            TargetParentNodeId = targetParentNodeId;
            Index = index;
        }
    }
}

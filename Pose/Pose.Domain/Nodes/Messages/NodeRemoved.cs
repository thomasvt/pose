namespace Pose.Domain.Nodes.Messages
{
    public class NodeRemoved
    {
        public ulong NodeId { get; }
        public ulong? ParentNodeId { get; }

        public NodeRemoved(ulong nodeId, ulong? parentNodeId)
        {
            NodeId = nodeId;
            ParentNodeId = parentNodeId;
        }
    }
}

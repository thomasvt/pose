namespace Pose.Domain.Documents.Messages
{
    /// <summary>
    /// Published just before a node is removed from the domain.
    /// </summary>
    public class NodeRemoving
    {
        public ulong NodeId { get; }

        public NodeRemoving(ulong nodeId)
        {
            NodeId = nodeId;
        }
    }
}

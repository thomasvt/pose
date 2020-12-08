namespace Pose.Domain.Editor.Messages
{
    public class NodeDeselected
    {
        public ulong NodeId { get; }

        public NodeDeselected(ulong nodeId)
        {
            NodeId = nodeId;
        }
    }
}

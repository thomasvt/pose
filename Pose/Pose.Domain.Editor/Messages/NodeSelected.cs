namespace Pose.Domain.Editor.Messages
{
    public class NodeSelected
    {
        public ulong NodeId { get; }

        public NodeSelected(ulong nodeId)
        {
            NodeId = nodeId;
        }
    }
}

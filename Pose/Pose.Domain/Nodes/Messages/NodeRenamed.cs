namespace Pose.Domain.Nodes.Messages
{
    public class NodeRenamed
    {
        public ulong NodeId { get; }
        public string Name { get; }

        public NodeRenamed(ulong nodeId, string name)
        {
            NodeId = nodeId;
            Name = name;
        }
    }
}

namespace Pose.Domain.Nodes.Messages
{
    public class BoneNodeAdded
    {
        public ulong NodeId { get; }
        public string Name { get; }

        public BoneNodeAdded(ulong nodeId, string name)
        {
            NodeId = nodeId;
            Name = name;
        }
    }
}

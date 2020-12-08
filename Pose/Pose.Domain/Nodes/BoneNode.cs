using Pose.Domain.Nodes.Properties;
using Pose.Framework.Messaging;

namespace Pose.Domain.Nodes
{
    public class BoneNode
    : Node
    {
        public BoneNode(IMessageBus messageBus, ulong nodeId, string name) : base(messageBus, nodeId, name)
        {
            AddProperty(PropertyType.BoneLength, 0f);
        }

        public override string ToString()
        {
            return GetLabel(Name);
        }

        public static string GetLabel(string name)
        {
            return $"Bone [{name}]";
        }
    }
}

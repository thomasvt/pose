using Pose.Domain.Nodes.Properties;
using Pose.Framework.Messaging;

namespace Pose.Domain.Nodes
{
    public class SpriteNode
    : Node
    {
        public SpriteNode(IMessageBus messageBus, ulong nodeId, string name, SpriteReference spriteRef)
        : base(messageBus, nodeId, name)
        {
            SpriteRef = spriteRef;
        }

        public SpriteReference SpriteRef { get; }

        public override string ToString()
        {
            return GetLabel(Name);
        }

        public static string GetLabel(string name)
        {
            return $"Sprite [{name}]";
        }
    }
}

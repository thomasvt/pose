using Pose.Domain.Documents.Messages;

namespace Pose.Domain.Nodes.Messages
{
    /// <summary>
    /// A SpriteNode was added. Parent attachment is communicated with a separate message <see cref="NodeAttachedToParent"/>.
    /// </summary>
    public class SpriteNodeAdded
    {
        public ulong NodeId { get; }
        public string Name { get; }
        public SpriteReference SpriteRef { get; }

        public SpriteNodeAdded(ulong nodeId, string name, SpriteReference spriteRef)
        {
            NodeId = nodeId;
            Name = name;
            SpriteRef = spriteRef;
        }
    }
}

using Pose.Domain.Documents.Messages;
using Pose.Framework.Messaging;

namespace Pose.Domain.Nodes
{
    /// <summary>
    /// The atomic modifications that can be performed. These should only be called by Events from History.
    /// </summary>
    public partial class NodeCollection
    {
        void IEditableNodeCollection.Attach(int index, Node node)
        {
            var sourceParentId = node.Parent?.Id;
            _children.Insert(index, node);
            (node as IEditableNode).SetOwner(_owner);
            _messageBus.Publish(new NodeAttachedToParent(node.Id, sourceParentId, _owner?.Id, index));
        }

        void IEditableNodeCollection.Detach(Node node)
        {
            _children.Remove(node);
        }
    }
}
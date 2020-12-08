using Pose.Domain.Documents;

namespace Pose.Domain.Nodes.Events
{
    internal class NodeRenamedEvent
    : IEvent
    {
        public ulong NodeId { get; }
        public string UndoName { get; }
        public string Name { get; }

        public NodeRenamedEvent(ulong nodeId, string undoName, string name)
        {
            NodeId = nodeId;
            UndoName = undoName;
            Name = name;
        }

        public void PlayForward(IEditableDocument document)
        {
            var node = document.GetNode(NodeId) as IEditableNode;
            node.Rename(Name);
        }

        public void PlayBackward(IEditableDocument document)
        {
            var node = document.GetNode(NodeId) as IEditableNode;
            node.Rename(UndoName);
        }
    }
}

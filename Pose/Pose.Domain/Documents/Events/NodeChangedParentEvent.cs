namespace Pose.Domain.Documents.Events
{
    internal class NodeChangedParentEvent
    : IEvent
    {
        public ulong NodeId { get; }
        public ulong? SourceParentNodeId { get; }
        public int UndoIndex { get; }
        public ulong? DestinationParentNodeId { get; }
        public int DestinationIndex { get; }

        public NodeChangedParentEvent(ulong nodeId, ulong? sourceParentNodeId, int undoIndex, ulong? destinationParentNodeId, int destinationIndex)
        {
            NodeId = nodeId;
            SourceParentNodeId = sourceParentNodeId;
            UndoIndex = undoIndex;
            DestinationParentNodeId = destinationParentNodeId;
            DestinationIndex = destinationIndex;
        }

        public void PlayForward(IEditableDocument document)
        {
            document.MoveNode(NodeId, DestinationParentNodeId, DestinationIndex);
        }

        public void PlayBackward(IEditableDocument document)
        {
            document.MoveNode(NodeId, SourceParentNodeId, UndoIndex);
        }
    }
}

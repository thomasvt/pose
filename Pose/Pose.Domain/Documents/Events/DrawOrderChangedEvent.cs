namespace Pose.Domain.Documents.Events
{
    internal class DrawOrderChangedEvent
    : IEvent
    {
        public ulong NodeId { get; }
        public int UndoIndex { get; }
        public int Index { get; }

        public DrawOrderChangedEvent(in ulong nodeId, int undoIndex, int index)
        {
            NodeId = nodeId;
            UndoIndex = undoIndex;
            Index = index;
        }

        public void PlayForward(IEditableDocument document)
        {
            document.MoveSpriteInFrontOf(NodeId, Index);
        }

        public void PlayBackward(IEditableDocument document)
        {
            document.MoveSpriteInFrontOf(NodeId, UndoIndex);
        }
    }
}

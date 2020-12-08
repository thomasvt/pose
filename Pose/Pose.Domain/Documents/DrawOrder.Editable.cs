using Pose.Domain.Documents.Messages;

namespace Pose.Domain.Documents
{
    /// <summary>
    /// The atomic modifications that can be performed. These should only be called by Events from History.
    /// </summary>
    public partial class DrawOrder
    {
        void IEditableDrawOrder.Remove(ulong nodeId)
        {
            _nodeIdsInOrder.Remove(nodeId);
        }

        void IEditableDrawOrder.Move(ulong nodeId, int index)
        {
            _nodeIdsInOrder.MoveSafe(nodeId, index);

            _messageBus.Publish(new DrawOrderChanged(nodeId, index));
        }

        void IEditableDrawOrder.AddInFront(in ulong nodeId)
        {
            _nodeIdsInOrder.Insert(0, nodeId);
        }
    }
}

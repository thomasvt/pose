namespace Pose.Domain.Documents
{
    internal interface IEditableDrawOrder
    {
        void Remove(ulong nodeId);
        void Move(ulong nodeId, int index);
        void AddInFront(in ulong nodeId);
    }
}

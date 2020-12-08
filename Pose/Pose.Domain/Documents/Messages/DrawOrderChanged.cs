namespace Pose.Domain.Documents.Messages
{
    /// <summary>
    /// A Node was moved to the index in the draworder.
    /// </summary>
    public class DrawOrderChanged
    {
        public ulong NodeId { get; }
        public int Index { get; }

        public DrawOrderChanged(ulong nodeId, int index)
        {
            NodeId = nodeId;
            Index = index;
        }
    }
}

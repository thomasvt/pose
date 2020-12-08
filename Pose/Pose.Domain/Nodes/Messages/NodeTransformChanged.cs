using Pose.Domain.Documents.Messages;

namespace Pose.Domain.Nodes.Messages
{
    public class NodeTransformChanged
    {
        public ulong NodeId { get; }
        /// <summary>
        /// See <see cref="BulkSceneUpdateEnded"/> for using this property.
        /// </summary>
        public bool IsBulkUpdate { get; }

        public NodeTransformChanged(ulong nodeId, bool isBulkUpdate)
        {
            NodeId = nodeId;
            IsBulkUpdate = isBulkUpdate;
        }
    }
}

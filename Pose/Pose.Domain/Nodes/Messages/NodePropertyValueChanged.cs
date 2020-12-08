using Pose.Domain.Nodes.Properties;

namespace Pose.Domain.Nodes.Messages
{
    public class NodePropertyValueChanged
    {
        public ulong NodeId { get; }
        public PropertyType PropertyType { get; }
        /// <summary>
        /// This property change is caused by a bulk operation on a large part of the scene. If this is True, you may want to ignore this message and wait for <see cref="BulkSceneUpdateEnded"/>
        /// </summary>
        public bool IsBulkUpdate { get; }

        public NodePropertyValueChanged(ulong nodeId, PropertyType propertyType, bool isBulkUpdate)
        {
            NodeId = nodeId;
            PropertyType = propertyType;
            IsBulkUpdate = isBulkUpdate;
        }
    }
}

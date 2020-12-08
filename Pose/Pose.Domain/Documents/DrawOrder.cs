using System;
using System.Collections.Generic;
using Pose.Framework.Messaging;

namespace Pose.Domain.Documents
{
    public partial class DrawOrder : IEditableDrawOrder
    {
        private readonly IMessageBus _messageBus;
        private readonly List<ulong> _nodeIdsInOrder;

        public DrawOrder(IMessageBus messageBus)
        {
            _messageBus = messageBus;
            _nodeIdsInOrder = new List<ulong>();
        }

        /// <summary>
        /// Node ids in draworder, front to back.
        /// </summary>
        public IList<ulong> GetNodeIdsInOrder()
        {
            return _nodeIdsInOrder.AsReadOnly();
        }

        public int IndexOf(ulong nodeId)
        {
            return _nodeIdsInOrder.IndexOf(nodeId);
        }

        internal void InternalSet(IEnumerable<ulong> nodeIdsInDrawOrder)
        {
            _nodeIdsInOrder.AddRange(nodeIdsInDrawOrder);
        }

        public int Count => _nodeIdsInOrder.Count;
    }
}

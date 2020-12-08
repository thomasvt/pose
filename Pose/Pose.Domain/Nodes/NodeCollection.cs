using System.Collections;
using System.Collections.Generic;
using Pose.Framework.Messaging;

namespace Pose.Domain.Nodes
{
    public partial class NodeCollection : IEditableNodeCollection, IEnumerable<Node>
    {
        private readonly IMessageBus _messageBus;
        private readonly Node _owner;
        private readonly List<Node> _children;

        public NodeCollection(IMessageBus messageBus, Node owner)
        {
            _messageBus = messageBus;
            _owner = owner;
            _children = new List<Node>();
        }


        public int IndexOf(Node node)
        {
            return _children.IndexOf(node);
        }

        public IEnumerator<Node> GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public int Count => _children.Count;

        /// <summary>
        /// Used by deserializer.
        /// </summary>
        internal void InternalAdd(Node node)
        {
            _children.Add(node);
        }
    }
}

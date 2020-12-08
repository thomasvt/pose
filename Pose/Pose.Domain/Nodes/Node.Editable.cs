using System.Collections.Generic;
using Pose.Domain.Nodes.Messages;
using Pose.Domain.Nodes.Properties;
using Pose.Framework.Messaging;

namespace Pose.Domain.Nodes
{
    /// <summary>
    /// The atomic modifications that can be performed. These should only be called by Events from History.
    /// </summary>
    public partial class Node
    {
        void IEditableNode.SetOwner(Node parent)
        {
            Parent = parent;
            OnTransformChanged();
        }
        
        void IEditableNode.Detach(Node node)
        {
            (Nodes as IEditableNodeCollection).Detach(node);
        }

        void IEditableNode.Attach(in int index, Node node)
        {
            (Nodes as IEditableNodeCollection).Attach(index, node);
        }

        void IEditableNode.SetPropertyValues(List<PropertyValueSet> propertyValues)
        {
            foreach (var set in propertyValues)
            {
                (GetProperty(set.PropertyType) as IEditableProperty).LoadFromPropertyValueSet(set);
            }
        }

        void IEditableNode.Rename(string name)
        {
            Name = name;
            MessageBus.Publish(new NodeRenamed(Id, name));
        }
    }
}

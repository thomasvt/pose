using System.Collections.Generic;
using Pose.Domain.Nodes.Properties;

namespace Pose.Domain.Nodes
{
    internal interface IEditableNode
    {
        void SetOwner(Node parent);
        void Attach(in int index, Node node);
        void Detach(Node node);
        void SetPropertyValues(List<PropertyValueSet> propertyValues);
        void Rename(string name);
    }
}
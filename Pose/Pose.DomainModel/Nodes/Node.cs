using System.Collections.Generic;
using System.Linq;

namespace Pose.DomainModel.Nodes
{
    public abstract class Node
        : Entity
    {
        private readonly List<IOperator> _operators;
        
        protected Node(long id, string name)
            : base(id)
        {
            Name = name;
            _operators = new List<IOperator>();
            Nodes = new NodeCollection(this);
        }

        public T GetFirstOperator<T>() where T : IOperator
        {
            return Operators.OfType<T>().First();
        }

        internal void AddOperator(IOperator @operator)
        {
            @operator.TransformChanged += (s, e) => OnTransformChanged();
            _operators.Add(@operator);
        }

        private void OnTransformChanged()
        {
            MessageBus.Publish(new NodeTransformChanged(this));
            foreach (var child in Nodes)
            {
                child.OnTransformChanged();
            }
        }

        public Matrix GetGlobalTransform()
        {
            var matrix = Matrix.Identity;
            foreach (var @operator in Operators)
            {
                @operator.ApplyTo(ref matrix);
            }

            if (Parent == null)
                return matrix;

            return Parent.GetGlobalTransform() * matrix;
        }

        internal void SetParent(Node parent)
        {
            if (parent == Parent)
                return;

            CorrectGlobalTransformForNewParent(parent);
            Parent = parent;
            OnTransformChanged();
        }

        internal void SetIsExpanded(bool isExpanded)
        {
            IsExpanded = isExpanded;
            MessageBus.Publish(new NodeIsExpandedChanged(this));
        }

        private void CorrectGlobalTransformForNewParent(Node newParentNode)
        {
            // todo temporary until shear and scale operators are added, maybe change to only correct translate+rotate ? See other apps.
            var globalTransform = GetGlobalTransform();
            Matrix correctedLocalTransform;

            if (newParentNode == null)
            {
                correctedLocalTransform = globalTransform;
            }
            else
            {
                var inverseNewParentTransform = newParentNode.GetGlobalTransform().GetInverse();
                if (!inverseNewParentTransform.HasValue)
                    return; // don't correct transform: it's mathematically not possible.

                correctedLocalTransform = inverseNewParentTransform.Value * globalTransform;
            }

            foreach (var @operator in Operators)
            {
                @operator.UpdateFromMatrix(ref correctedLocalTransform);
            }
        }

        public Node Parent { get; private set; }

        public string Name { get; }

        public IReadOnlyCollection<IOperator> Operators => _operators;

        public bool IsExpanded { get; private set; }

        public NodeCollection Nodes { get; }
    }
}

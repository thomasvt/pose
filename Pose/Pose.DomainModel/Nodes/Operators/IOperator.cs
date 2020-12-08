using System;

namespace Pose.DomainModel.Nodes.Operators
{
    public interface IOperator
    {
        void ApplyTo(ref Matrix matrix);
        void UpdateFromMatrix(ref Matrix transform);

        event EventHandler TransformChanged;
    }
}
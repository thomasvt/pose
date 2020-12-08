using System;

namespace Pose.DomainModel.Nodes.Operators
{
    public class ScaleOperator : IOperator
    {
        private Vector2 _scale;

        public ScaleOperator()
        {
            _scale = Vector2.One;
        }

        public Vector2 Scale
        {
            get => _scale;
            internal set
            {
                if (_scale != value)
                {
                    _scale = value;
                    TransformChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public void ApplyTo(ref Matrix matrix)
        {
            matrix.M11 *= Scale.X;
            matrix.M22 *= Scale.Y;
        }

        void IOperator.UpdateFromMatrix(ref Matrix transform)
        {
            
        }

        public event EventHandler TransformChanged;
    }
}

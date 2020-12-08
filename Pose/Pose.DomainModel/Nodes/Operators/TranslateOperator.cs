using System;

namespace Pose.DomainModel.Nodes.Operators
{
    public class TranslateOperator : IOperator
    {
        private Vector2 _translation;

        internal void SetX(in float x)
        {
            Translation = new Vector2(x, _translation.Y);
        }

        internal void SetY(in float y)
        {
            Translation = new Vector2(_translation.X, y);
        }

        public Vector2 Translation
        {
            get => _translation;
            internal set
            {
                if (_translation != value)
                {
                    _translation = value;
                    TransformChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        void IOperator.UpdateFromMatrix(ref Matrix transform)
        {
            Translation = new Vector2(transform.M13, transform.M23);
        }

        public void ApplyTo(ref Matrix matrix)
        {
            var m13 = Translation.X;
            var m23 = Translation.Y;

            matrix.M13 = matrix.M11 * m13 + matrix.M12 * m23 + matrix.M13;
            matrix.M23 = matrix.M21 * m13 + matrix.M22 * m23 + matrix.M23;
            matrix.M33 = matrix.M31 * m13 + matrix.M32 * m23 + matrix.M33;
        }

        public event EventHandler TransformChanged;
    }
}

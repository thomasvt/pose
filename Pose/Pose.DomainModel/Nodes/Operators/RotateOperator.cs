using System;

namespace Pose.DomainModel.Nodes.Operators
{
    public class RotateOperator: IOperator
    {
        // cos, -sin, 0
        // sin, cos,  0
        // 0,   0,    1 

        private const float Pi2 = MathF.PI * 2f;
        private float _angle;

        /// <summary>
        /// Angle in radians.
        /// </summary>
        public float Angle
        {
            get => _angle;
            internal set
            {
                if (_angle != value)
                {
                    _angle = value;
                    TransformChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public void ApplyTo(ref Matrix matrix)
        {
            var sin = MathF.Sin(Angle);
            var cos = MathF.Cos(Angle);

            // below is just normal matrix multiplication, but stripped of unneeded parts (because the rotation matrix has many 0 and 1s) and optimized to assign results directly to the ref Matrix.

            var m11 = matrix.M11;
            matrix.M11 = m11 * cos + matrix.M12 * sin;
            matrix.M12 = m11 * -sin + matrix.M12 * cos;

            var m21 = matrix.M21;
            matrix.M21 = m21 * cos + matrix.M22 * sin;
            matrix.M22 = m21 * -sin + matrix.M22 * cos;

            var m31 = matrix.M31;
            matrix.M31 = m31 * cos + matrix.M32 * sin;
            matrix.M32 = m31 * -sin + matrix.M32 * cos;
        }

        void IOperator.UpdateFromMatrix(ref Matrix transform)
        {
            // todo replace with angle parent-chain addition because reversing the matrix is deadend, and had rounding error issues (eg >1 )
            var m11 = transform.M11;
            if (m11 < 0f)
            {
                m11 = 0;
            }
            else if (m11 > 1f)
            {
                m11 = 1f;
            }
                
            var acos = MathF.Acos(m11);
            if (transform.M21 >= 0)
            {
                Angle = acos;
            }
            else
            {
                Angle = Pi2 - acos;
            }
        }

        public event EventHandler TransformChanged;
    }
}

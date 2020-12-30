using System;

namespace Pose.Common
{
    public struct Matrix
    : IEquatable<Matrix>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m11">row 1, col 1</param>
        /// <param name="m12">row 1, col 2</param>
        /// <param name="m13">row 1, col 3</param>
        /// <param name="m21">row 2, col 1</param>
        /// <param name="m22">row 2, col 2</param>
        /// <param name="m23">row 2, col 3</param>
        /// <param name="m31">row 3, col 1</param>
        /// <param name="m32">row 3, col 2</param>
        /// <param name="m33">row 3, col 3</param>
        public Matrix(float m11, float m12, float m13, float m21, float m22, float m23, float m31, float m32, float m33)
        {
            M11 = m11;
            M12 = m12;
            M13 = m13;

            M21 = m21;
            M22 = m22;
            M23 = m23;

            M31 = m31;
            M32 = m32;
            M33 = m33;
        }

        /// <summary>
        /// row 1, col 1
        /// </summary>
        public float M11;
        /// <summary>
        /// row 1, col 2
        /// </summary>
        public float M12;
        /// <summary>
        /// row 1, col 3
        /// </summary>
        public float M13;
        /// <summary>
        /// row 2, col 1
        /// </summary>
        public float M21;
        /// <summary>
        /// row 2, col 2
        /// </summary>
        public float M22;
        /// <summary>
        /// row 2, col 3
        /// </summary>
        public float M23;
        /// <summary>
        /// row 3, col 1
        /// </summary>
        public float M31;
        /// <summary>
        /// row 3, col 2
        /// </summary>
        public float M32;
        /// <summary>
        /// row 3 col 3
        /// </summary>
        public float M33;

        public static Matrix Identity = new Matrix
        {
            M11 = 1f,
            M22 = 1f,
            M33 = 1f,
        };

        public static Matrix operator*(Matrix a, Matrix b)
        {
            return new Matrix
            {
                M11 = a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31,
                M12 = a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32,
                M13 = a.M11 * b.M13 + a.M12 * b.M23 + a.M13 * b.M33,

                M21 = a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31,
                M22 = a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32,
                M23 = a.M21 * b.M13 + a.M22 * b.M23 + a.M23 * b.M33,

                M31 = a.M31 * b.M11 + a.M32 * b.M21 + a.M33 * b.M31,
                M32 = a.M31 * b.M12 + a.M32 * b.M22 + a.M33 * b.M32,
                M33 = a.M31 * b.M13 + a.M32 * b.M23 + a.M33 * b.M33,
            };
        }

        public Vector2 TransformDistance(Vector2 vector)
        {
            return new Vector2(M11 * vector.X + M12 * vector.Y, M21 * vector.X + M22 * vector.Y);
        }

        public override string ToString()
        {
            return $"{M11}, {M12}, {M13} | {M21}, {M22}, {M23} | {M31}, {M32}, {M33}";
        }

        public float GetDeterminant() => M11 * (M22 * M33 - M23 * M32) - M12 * (M21 * M33 - M23 * M31) + M13 * (M21 * M32 - M22 * M31);

        public Matrix? GetInverse()
        {
            // https://www.wikihow.com/Find-the-Inverse-of-a-3x3-Matrix#:~:text=Divide%20each%20term%20of%20the%20adjugate%20matrix%20by%20the%20determinant.&text=Place%20the%20result%20of%20each,inverse%20of%20the%20original%20matrix.

            var det = M11 * (M22 * M33 - M23 * M32) - M12 * (M21 * M33 - M23 * M31) + M13 * (M21 * M32 - M22 * M31);
            if (det == 0f)
                return null; // no inverse

            // transpose
            var m11 = M11;
            var m12 = M21;
            var m13 = M31;
            var m21 = M12;
            var m22 = M22;
            var m23 = M32;
            var m31 = M13;
            var m32 = M23;
            var m33 = M33;

            // adjugate
            var det11 = m22 * m33 - m23 * m32;
            var det12 = -m21 * m33 + m23 * m31; // -
            var det13 = m21 * m32 - m22 * m31;

            var det21 = -m12 * m33 + m32 * m13; // -
            var det22 = m11 * m33 - m13 * m31;
            var det23 = -m11 * m32 + m12 * m31; // -

            var det31 = m12 * m23 - m13 * m22;
            var det32 = -m11 * m23 + m13 * m21; // -
            var det33 = m11 * m22 - m12 * m21;

            // inverse
            var invDet = 1f / det;

            return new Matrix(det11 * invDet, det12 * invDet, det13 * invDet, det21 * invDet, det22 * invDet, det23 * invDet, det31 * invDet, det32 * invDet, det33 * invDet);
        }

        public bool Equals(Matrix other)
        {
            return M11.Equals(other.M11) && M12.Equals(other.M12) && M13.Equals(other.M13) && M21.Equals(other.M21) && M22.Equals(other.M22) && M23.Equals(other.M23) && M31.Equals(other.M31) && M32.Equals(other.M32) && M33.Equals(other.M33);
        }

        public override bool Equals(object obj)
        {
            return obj is Matrix other && Equals(other);
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(M11);
            hashCode.Add(M12);
            hashCode.Add(M13);
            hashCode.Add(M21);
            hashCode.Add(M22);
            hashCode.Add(M23);
            hashCode.Add(M31);
            hashCode.Add(M32);
            hashCode.Add(M33);
            return hashCode.ToHashCode();
        }

        public Vector2 GetTranslation()
        {
            return new Vector2(M13, M23);
        }
    }
}

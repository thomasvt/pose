using System.Diagnostics.Contracts;

namespace Pose.Common.Curves
{
    public struct Polynomial3
    {
        public readonly float A, B, C, D;

        public Polynomial3(float a, float b, float c, float d)
        {
            A = a;
            B = b;
            C = c;
            D = d;
        }

        [Pure]
        public Polynomial2 GetDerivative()
        {
            return new Polynomial2(3f*A, 2f*B, C);
        }

        /// <summary>
        /// Calculate f(t) = A*t^3 + B*t^2 + C*t + D
        /// </summary>
        [Pure]
        public float Solve(float t)
        {
            var t2 = t * t;
            var t3 = t * t2;
            return A * t3 + B * t2 + C * t + D;
        }
    }
}

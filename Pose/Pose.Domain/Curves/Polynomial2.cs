namespace Pose.Domain.Curves
{
    public struct Polynomial2
    {
        public readonly float A, B, C;

        public Polynomial2(float a, float b, float c)
        {
            A = a;
            B = b;
            C = c;
        }

        /// <summary>
        /// Calculate f(t) = A*t^2 + B*t + C
        /// </summary>
        public float Calc(float t)
        {
            var t2 = t * t;
            return A * t2 + B * t + C;
        }
    }
}

using System.Windows;
using Pose.Domain;

namespace Pose
{
    public static class VectorExtensions
    {
        public static Vector ToVector(this Point point)
        {
            return new Vector(point.X, point.Y);
        }

        public static Vector ToVector(this Vector2 v)
        {
            return new Vector(v.X, v.Y);
        }

        public static Vector2 ToVector2(this Vector vector)
        {
            return new Vector2((float)vector.X, (float)vector.Y);
        }

        public static Point ToPoint(this Vector2 vector)
        {
            return new Point(vector.X, vector.Y);
        }

        public static Point ToPoint(this Vector vector)
        {
            return new Point(vector.X, vector.Y);
        }
    }
}

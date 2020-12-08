using System;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Pose.Domain
{
    public readonly struct Vector2
    : IEquatable<Vector2>
    {
        public readonly float X;
        public readonly float Y;

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static bool operator ==(Vector2 a, Vector2 b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Vector2 a, Vector2 b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X - b.X, a.Y - b.Y);
        }

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2 operator *(Vector2 a, float factor)
        {
            return new Vector2(a.X * factor, a.Y * factor);
        }

        public static Vector2 operator /(Vector2 a, float factor)
        {
            return new Vector2(a.X / factor, a.Y / factor);
        }

        public bool Equals(Vector2 other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        public override bool Equals(object obj)
        {
            return obj is Vector2 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public float Dot(in Vector2 b)
        {
            return X * b.X + Y * b.Y;
        }

        public Vector2 GetPerpendicular()
        {
            return new Vector2(-Y, X);
        }

        public Vector2 Normalized()
        {
            var magnitude = Magnitude;
            return new Vector2(X / magnitude, Y / magnitude);
        }

        public float GetAngle()
        {
            return MathF.Atan2(Y, X);
        }

        public override string ToString()
        {
            return $"{X:0.00}, {Y:0.00}";
        }

        public static Vector2 Zero = new Vector2(0, 0);
        public static Vector2 One = new Vector2(1f, 1f);
        public float Magnitude => MathF.Sqrt(X * X + Y * Y);

        public static Vector2 FromAngle(in float angle, in float length)
        {
            return new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * length;
        }
    }
}

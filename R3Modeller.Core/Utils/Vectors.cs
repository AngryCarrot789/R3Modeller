using System;
using System.Numerics;

namespace R3Modeller.Core.Utils {
    public static class Vectors {
        public static readonly Vector2 Zero = Vector2.Zero;
        public static readonly Vector2 One = Vector2.One;
        public static readonly Vector2 MinValue = new Vector2(float.MinValue);
        public static readonly Vector2 MaxValue = new Vector2(float.MaxValue);

        public static Vector2 Clamp(this Vector2 a, Vector2 min, Vector2 max) => Vector2.Clamp(a, min, max);
        public static Vector3 Clamp(this Vector3 a, Vector3 min, Vector3 max) => Vector3.Clamp(a, min, max);
        public static Vector4 Clamp(this Vector4 a, Vector4 min, Vector4 max) => Vector4.Clamp(a, min, max);

        public static Vector2 Round(this Vector2 vector, int digits) {
            return new Vector2((float) Math.Round(vector.X, digits), (float) Math.Round(vector.Y, digits));
        }

        public static Vector3 Round(this Vector3 vector, int digits) {
            return new Vector3((float) Math.Round(vector.X, digits), (float) Math.Round(vector.Y, digits), (float) Math.Round(vector.Z, digits));
        }

        public static bool IsPositiveInfinityX(this Vector2 vector) => float.IsPositiveInfinity(vector.X);

        public static bool IsPositiveInfinityY(this Vector2 vector) => float.IsPositiveInfinity(vector.Y);

        public static bool IsNegativeInfinityX(this Vector2 vector) => float.IsNegativeInfinity(vector.X);

        public static bool IsNegativeInfinityY(this Vector2 vector) => float.IsNegativeInfinity(vector.Y);

        public static Vector2 Lerp(in this Vector2 a, in Vector2 b, float blend) {
            return new Vector2(blend * (b.X - a.X) + a.X, blend * (b.Y - a.Y) + a.Y);
        }

        public static Vector2 WithX(this Vector2 v, float x) => new Vector2(x, v.Y);
        public static Vector2 WithY(this Vector2 v, float y) => new Vector2(v.X, y);
        public static Vector3 WithX(this Vector3 v, float x) => new Vector3(x, v.Y, v.Z);
        public static Vector3 WithY(this Vector3 v, float y) => new Vector3(v.X, y, v.Z);
        public static Vector3 WithZ(this Vector3 v, float z) => new Vector3(v.X, v.Y, z);
        public static Vector4 WithX(this Vector4 v, float x) => new Vector4(x, v.Y, v.Z, v.W);
        public static Vector4 WithY(this Vector4 v, float y) => new Vector4(v.X, y, v.Z, v.W);
        public static Vector4 WithZ(this Vector4 v, float z) => new Vector4(v.X, v.Y, z, v.W);
        public static Vector4 WithW(this Vector4 v, float w) => new Vector4(v.X, v.Y, v.Z, w);
    }
}
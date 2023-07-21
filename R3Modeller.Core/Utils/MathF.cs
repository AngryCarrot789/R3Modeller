using System;
using System.Runtime.CompilerServices;

namespace R3Modeller.Core.Utils {
    public static class MathF {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Abs(float value) => Math.Abs(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Acos(float d) => (float)Math.Acos(d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Asin(float d) => (float)Math.Asin(d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Atan(float d) => (float)Math.Atan(d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Atan2(float y, float x) => (float)Math.Atan2(y, x);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Ceiling(float a) => (float)Math.Ceiling(a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cos(float d) => (float)Math.Cos(d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cosh(float value) => (float)Math.Cosh(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Exp(float d) => (float)Math.Exp(d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Floor(float d) => (float)Math.Floor(d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float IEEERemainder(float x, float y) => (float)Math.IEEERemainder(x, y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Log(float a, float newBase) => (float)Math.Log(a, newBase);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Log(float d) => (float)Math.Log(d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Log10(float d) => (float)Math.Log10(d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Max(float a, float b) => Math.Max(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Min(float a, float b) => Math.Min(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Pow(float x, float y) => (float)Math.Pow(x, y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Round(float value, int digits, MidpointRounding mode) => (float)Math.Round(value, digits, mode);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Round(float value, MidpointRounding mode) => (float)Math.Round(value, mode);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Round(float value, int digits) => (float)Math.Round(value, digits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Round(float a) => (float)Math.Round(a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sign(float v) => Math.Sign(v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sin(float a) => (float)Math.Sin(a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sinh(float v) => (float)Math.Sinh(v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sqrt(float d) => (float)Math.Sqrt(d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Tan(float a) => (float)Math.Tan(a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Tanh(float value) => (float)Math.Tanh(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Truncate(float d) => (float)Math.Truncate(d);
    }
}
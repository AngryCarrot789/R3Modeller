using System;
using System.Runtime.CompilerServices;

namespace R3Modeller.Core.Utils {
    public static class MathF {
        private static IMathF instance = new CastingMathF();

        public static IMathF Instance {
            get => instance;
            set => instance = value ?? throw new ArgumentNullException(nameof(value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Acos(float f) =>           instance.Acos(f);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Asin(float f) =>           instance.Asin(f);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Atan(float f) =>           instance.Atan(f);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Atan2(float y, float x) => instance.Atan2(y, x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Ceiling(float a) =>        instance.Ceiling(a);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Cos(float f) =>            instance.Cos(f);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Cosh(float value) =>       instance.Cosh(value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Floor(float f) =>          instance.Floor(f);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Sin(float a) =>            instance.Sin(a);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Tan(float a) =>            instance.Tan(a);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Sinh(float value) =>       instance.Sinh(value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Tanh(float value) =>       instance.Tanh(value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Round(float a) =>          instance.Round(a);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Sqrt(float f) =>           instance.Sqrt(f);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Log(float f) =>            instance.Log(f);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Log10(float f) =>          instance.Log10(f);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Exp(float f) =>            instance.Exp(f);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Pow(float x, float y) =>   instance.Pow(x, y);

        private class CastingMathF : IMathF {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] float IMathF.Acos(float f) => (float) Math.Acos(f);
            [MethodImpl(MethodImplOptions.AggressiveInlining)] float IMathF.Asin(float f) => (float) Math.Asin(f);
            [MethodImpl(MethodImplOptions.AggressiveInlining)] float IMathF.Atan(float f) => (float) Math.Atan(f);
            [MethodImpl(MethodImplOptions.AggressiveInlining)] float IMathF.Atan2(float y, float x) => (float) Math.Atan2(y, x);
            [MethodImpl(MethodImplOptions.AggressiveInlining)] float IMathF.Ceiling(float a) => (float) Math.Ceiling(a);
            [MethodImpl(MethodImplOptions.AggressiveInlining)] float IMathF.Cos(float f) => (float) Math.Cos(f);
            [MethodImpl(MethodImplOptions.AggressiveInlining)] float IMathF.Cosh(float value) => (float) Math.Cosh(value);
            [MethodImpl(MethodImplOptions.AggressiveInlining)] float IMathF.Floor(float f) => (float) Math.Floor(f);
            [MethodImpl(MethodImplOptions.AggressiveInlining)] float IMathF.Sin(float a) => (float) Math.Sin(a);
            [MethodImpl(MethodImplOptions.AggressiveInlining)] float IMathF.Tan(float a) => (float) Math.Tan(a);
            [MethodImpl(MethodImplOptions.AggressiveInlining)] float IMathF.Sinh(float value) => (float) Math.Sinh(value);
            [MethodImpl(MethodImplOptions.AggressiveInlining)] float IMathF.Tanh(float value) => (float) Math.Tanh(value);
            [MethodImpl(MethodImplOptions.AggressiveInlining)] float IMathF.Round(float a) => (float) Math.Round(a);
            [MethodImpl(MethodImplOptions.AggressiveInlining)] float IMathF.Sqrt(float f) => (float) Math.Sqrt(f);
            [MethodImpl(MethodImplOptions.AggressiveInlining)] float IMathF.Log(float f) => (float) Math.Log(f);
            [MethodImpl(MethodImplOptions.AggressiveInlining)] float IMathF.Log10(float f) => (float) Math.Log10(f);
            [MethodImpl(MethodImplOptions.AggressiveInlining)] float IMathF.Exp(float f) => (float) Math.Exp(f);
            [MethodImpl(MethodImplOptions.AggressiveInlining)] float IMathF.Pow(float x, float y) => (float) Math.Pow(x, y);
        }
    }
}
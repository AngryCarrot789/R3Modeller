namespace R3Modeller.Core.Utils {
    /// <summary>
    /// An interface for implementing floating point math operations
    /// </summary>
    public interface IMathF {
        float Acos(float f);
        float Asin(float f);
        float Atan(float f);
        float Atan2(float y, float x);
        float Ceiling(float a);
        float Cos(float f);
        float Cosh(float value);
        float Floor(float f);
        float Sin(float a);
        float Tan(float a);
        float Sinh(float value);
        float Tanh(float value);
        float Round(float a);
        float Sqrt(float f);
        float Log(float f);
        float Log10(float f);
        float Exp(float f);
        float Pow(float x, float y);
    }
}
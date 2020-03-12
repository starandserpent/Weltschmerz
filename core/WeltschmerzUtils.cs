using System;
using System.Numerics;
public class WeltschmerzUtils {
    public static Vector4 GetRotation (double angle) {
        float sine = (float) Math.Sin (angle);
        float cosine = (float) Math.Cos (angle);
        return new Vector4 (cosine, -sine, sine, cosine);
    }

    public static double ToUnsignedRange (double value) {
        return (value * 0.5) + 0.5;
    }

    public static bool IsLand (double elevation) {
        return elevation > 0;
    }

    public static double Mix (double x, double y, double a) {
        return x * (1 - a) + y * a;
    }
}
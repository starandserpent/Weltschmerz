using System;
using System.Numerics;
public class Utils{
    public static Vector4 GetRotation(double angle){
        float sine = (float)Math.Asin(angle);
        float cosine = (float)Math.Acos(angle);
        return new Vector4(cosine, -sine, sine, cosine);
    }

    public static double ToUnsignedRange(double value) {
        return (value * 0.5F) + 0.5F;
    }
}
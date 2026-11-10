namespace Weltschmerz.Spatial.Climate;

/// <summary>Stable, signed-coordinate two-dimensional value noise for offline atlas fields.</summary>
internal static class EnvironmentalValueNoise {
    private const double LargestSafeCellCoordinate = 9_223_372_036_854_774_784d;

    internal static double Sample(long seed, ulong salt, double xMetres, double zMetres, double spacingMetres) {
        var x = xMetres / spacingMetres;
        var z = zMetres / spacingMetres;
        var floorX = Math.Floor(x);
        var floorZ = Math.Floor(z);
        if (floorX < long.MinValue || floorX >= LargestSafeCellCoordinate ||
            floorZ < long.MinValue || floorZ >= LargestSafeCellCoordinate)
            throw new ArgumentOutOfRangeException(nameof(xMetres), "Atlas noise coordinates exceed the signed 64-bit lattice range.");
        var ix = (long)floorX;
        var iz = (long)floorZ;
        var fx = Smoothstep(x - floorX);
        var fz = Smoothstep(z - floorZ);
        var north = Lerp(Value(seed, salt, ix, iz), Value(seed, salt, checked(ix + 1), iz), fx);
        var south = Lerp(Value(seed, salt, ix, checked(iz + 1)),
            Value(seed, salt, checked(ix + 1), checked(iz + 1)), fx);
        return Lerp(north, south, fz);
    }

    private static double Value(long seed, ulong salt, long x, long z) =>
        (Mix(unchecked((ulong)seed) ^ salt ^ unchecked((ulong)x * 0xD6E8FEB86659FD93UL) ^
             unchecked((ulong)z * 0xA5A3564E27F8862BUL)) >> 11) * (1d / 9_007_199_254_740_992d);

    private static ulong Mix(ulong value) {
        value += 0x9E3779B97F4A7C15UL;
        value = (value ^ (value >> 30)) * 0xBF58476D1CE4E5B9UL;
        value = (value ^ (value >> 27)) * 0x94D049BB133111EBUL;
        return value ^ (value >> 31);
    }

    private static double Smoothstep(double amount) => amount * amount * (3d - 2d * amount);
    private static double Lerp(double first, double second, double amount) => first + (second - first) * amount;
}

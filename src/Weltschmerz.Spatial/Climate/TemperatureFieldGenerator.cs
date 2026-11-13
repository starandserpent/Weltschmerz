using Weltschmerz.Spatial.Atlas;
using System.Threading;

namespace Weltschmerz.Spatial.Climate;

internal static class TemperatureFieldGenerator {
    private const ulong TemperatureSalt = 0x54454D5041545552UL;

    internal static double[] Generate(
        EnvironmentalAtlasDefinition definition,
        ReadOnlySpan<double> elevations,
        CancellationToken cancellationToken) {
        var result = new double[elevations.Length];
        var profile = definition.Profile;
        for (var z = 0; z < definition.Bounds.Height; z++) for (var x = 0; x < definition.Bounds.Width; x++) {
            if ((x & 0x3f) == 0) cancellationToken.ThrowIfCancellationRequested();
            var index = checked(z * definition.Bounds.Width + x);
            var (worldX, worldZ) = definition.Bounds.CellCenter(x, z);
            var latitudeAdjustment = profile.MaximumNorthSouthTemperatureAdjustmentCelsius *
                Math.Min(Math.Abs(worldZ) / profile.NorthSouthAdjustmentDistanceMetres, 1d);
            var noise = EnvironmentalValueNoise.Sample(definition.WorldSeed, TemperatureSalt,
                worldX, worldZ, profile.TemperatureNoiseSpacingMetres);
            result[index] = profile.NominalSeaLevelTemperatureCelsius - latitudeAdjustment +
                (noise - 0.5d) * profile.TemperatureNoiseAmplitudeCelsius * 2d -
                Math.Max(elevations[index], 0d) * profile.TemperatureLapseRateCelsiusPerMetre;
        }
        return result;
    }
}

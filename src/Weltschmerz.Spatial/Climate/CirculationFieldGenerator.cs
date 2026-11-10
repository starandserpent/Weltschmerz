using Weltschmerz.Spatial.Atlas;

namespace Weltschmerz.Spatial.Climate;

internal static class CirculationFieldGenerator {
    private const ulong CirculationXSalt = 0x43495243554C5831UL;
    private const ulong CirculationZSalt = 0x43495243554C5A31UL;

    internal static (double[] Pressure, double[] X, double[] Z) Generate(
        EnvironmentalAtlasDefinition definition,
        ReadOnlySpan<double> elevations,
        ReadOnlySpan<double> temperatures) {
        var pressure = new double[elevations.Length];
        var xField = new double[elevations.Length];
        var zField = new double[elevations.Length];
        var profile = definition.Profile;
        for (var z = 0; z < definition.Bounds.Height; z++) for (var x = 0; x < definition.Bounds.Width; x++) {
            var index = checked(z * definition.Bounds.Width + x);
            var (worldX, worldZ) = definition.Bounds.CellCenter(x, z);
            // Hydrostatic pressure is bounded at the low end to keep extreme supplied elevations finite.
            var pressureExponent = Math.Clamp(-elevations[index] / profile.PressureScaleHeightMetres, -20d, 20d);
            var temperaturePressureFactor = Math.Clamp(288.15d /
                (temperatures[index] + 273.15d), 0.65d, 1.5d);
            pressure[index] = profile.ReferenceAtmosphericPressurePascals *
                Math.Exp(pressureExponent) * temperaturePressureFactor;

            var eastward = EnvironmentalValueNoise.Sample(definition.WorldSeed, CirculationXSalt,
                worldX, worldZ, profile.WindVariationSpacingMetres);
            var northSouth = EnvironmentalValueNoise.Sample(definition.WorldSeed, CirculationZSalt,
                worldX, worldZ, profile.WindVariationSpacingMetres);
            xField[index] = 0.65d + eastward * 0.35d;
            zField[index] = (northSouth - 0.5d) * 0.6d;
        }
        return (pressure, xField, zField);
    }
}

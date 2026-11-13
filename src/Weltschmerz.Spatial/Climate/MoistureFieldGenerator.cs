using Weltschmerz.Spatial.Atlas;
using System.Threading;

namespace Weltschmerz.Spatial.Climate;

internal static class MoistureFieldGenerator {
    internal static (double[] Humidity, double[] SurfaceMoisture, double[] Precipitation) Generate(
        EnvironmentalAtlasDefinition definition,
        ReadOnlySpan<double> elevations,
        ReadOnlySpan<double> circulationX,
        ReadOnlySpan<double> circulationZ,
        CancellationToken cancellationToken) {
        var count = elevations.Length;
        var humidity = new double[count];
        var surfaceMoisture = new double[count];
        var precipitation = new double[count];
        var width = definition.Bounds.Width;
        var height = definition.Bounds.Height;
        var profile = definition.Profile;

        // The fixed west-to-east traversal is the only transport pass. No data is read beyond atlas bounds.
        for (var x = 0; x < width; x++) for (var z = 0; z < height; z++) {
            if ((z & 0x3f) == 0) cancellationToken.ThrowIfCancellationRequested();
            var index = checked(z * width + x);
            var (worldX, worldZ) = definition.Bounds.CellCenter(x, z);
            var previousHumidity = profile.WesternBoundaryHumidity;
            var previousElevation = elevations[index];
            if (x > 0) {
                var upstreamZ = Math.Clamp(z - circulationZ[index] / circulationX[index], 0d, height - 1d);
                var lowZ = (int)Math.Floor(upstreamZ);
                var highZ = Math.Min(lowZ + 1, height - 1);
                var blend = upstreamZ - lowZ;
                var westLowIndex = checked(lowZ * width + x - 1);
                var westHighIndex = checked(highZ * width + x - 1);
                previousHumidity = Lerp(humidity[westLowIndex], humidity[westHighIndex], blend);
                previousElevation = Lerp(elevations[westLowIndex], elevations[westHighIndex], blend);
            }

            var elevationRise = Math.Max(0d, elevations[index] - previousElevation);
            var oceanSource = elevations[index] <= definition.OceanElevationMetres;
            var sourceMoisture = oceanSource
                ? profile.OceanEvaporationContribution
                : profile.LandMoistureContribution;
            var advected = previousHumidity * profile.UpwindHumidityRetention + sourceMoisture;
            var available = Math.Clamp(advected, 0d, 1d);
            var orographic = Math.Min(profile.MaximumOrographicPrecipitationMillimetres,
                elevationRise * profile.OrographicPrecipitationPerMetreRiseMillimetres * available);
            var noise = PrecipitationFieldGenerator.SampleNoise(definition, worldX, worldZ);
            var annualPrecipitation = Math.Min(profile.MaximumAnnualPrecipitationMillimetres,
                profile.BaselinePrecipitationMillimetres +
                noise * profile.NoisePrecipitationVariationMillimetres +
                available * profile.HumidityPrecipitationContributionMillimetres + orographic);

            var precipitationLoss = Math.Min(profile.PrecipitationMoistureLoss,
                annualPrecipitation / profile.MaximumAnnualPrecipitationMillimetres * profile.PrecipitationMoistureLoss);
            humidity[index] = Math.Clamp(available * (1d - precipitationLoss), 0d, 1d);
            precipitation[index] = annualPrecipitation;
            surfaceMoisture[index] = Math.Clamp(humidity[index] * 0.65d +
                annualPrecipitation / profile.MaximumAnnualPrecipitationMillimetres * 0.35d, 0d, 1d);
        }
        return (humidity, surfaceMoisture, precipitation);
    }

    private static double Lerp(double from, double to, double amount) => from + (to - from) * amount;
}

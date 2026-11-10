using Weltschmerz.Spatial.Atlas;

namespace Weltschmerz.Spatial.Biomes;

/// <summary>Maps long-term climate normals to the biome identity defined by a spatial profile.</summary>
public static class SpatialBiomeClassifier {
    public static SpatialBiomeId Classify(
        EnvironmentalAtlasProfile profile,
        double temperatureCelsius,
        double annualPrecipitationMillimeters) {
        ArgumentNullException.ThrowIfNull(profile);
        if (!double.IsFinite(temperatureCelsius)) throw new ArgumentOutOfRangeException(nameof(temperatureCelsius));
        if (!double.IsFinite(annualPrecipitationMillimeters))
            throw new ArgumentOutOfRangeException(nameof(annualPrecipitationMillimeters));

        return temperatureCelsius >= profile.ForestMinimumTemperatureCelsius &&
               temperatureCelsius <= profile.ForestMaximumTemperatureCelsius &&
               annualPrecipitationMillimeters >= profile.ForestPrecipitationThresholdMillimetres
            ? SpatialBiomeIds.TemperateForest
            : SpatialBiomeIds.Meadow;
    }
}

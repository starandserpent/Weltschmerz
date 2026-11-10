using Weltschmerz.Spatial.Atlas;
using Weltschmerz.Spatial.Biomes;

namespace Weltschmerz.Spatial.Tests;

public sealed class SpatialBiomeClassificationTests {
    [Theory]
    [InlineData(-5d, 850d, 1)]
    [InlineData(30d, 850d, 1)]
    [InlineData(12d, 849.999d, 2)]
    [InlineData(-5.001d, 1_900d, 2)]
    [InlineData(30.001d, 1_900d, 2)]
    public void Temperate_forest_uses_inclusive_configured_thresholds(
        double temperature,
        double precipitation,
        ushort expectedBiome) {
        var actual = SpatialBiomeClassifier.Classify(
            EnvironmentalAtlasProfile.TemperateAtlasV1,
            temperature,
            precipitation);

        Assert.Equal(new SpatialBiomeId(expectedBiome), actual);
    }

    [Fact]
    public void Zero_is_not_a_valid_biome_identity() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new SpatialBiomeId(0));
}

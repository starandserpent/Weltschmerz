using Weltschmerz.Spatial.Atlas;
using Weltschmerz.Spatial.Biomes;
using Weltschmerz.Spatial.Climate;

namespace Weltschmerz.Spatial.Tests;

public sealed class EnvironmentalAtlasGeneratorTests {
    [Fact]
    public void Complete_atlas_generation_is_deterministic_and_row_major() {
        var definition = Definition(seed: 41, width: 18, height: 12);
        var elevations = Enumerable.Range(0, definition.ExpectedCellCount)
            .Select(index => index % 17 == 0 ? 640d : -0.5d).ToArray();
        var generator = new EnvironmentalAtlasGenerator();

        var first = generator.Generate(definition, elevations);
        var second = generator.Generate(definition, elevations);

        Assert.Equal(definition.ExpectedCellCount, first.Cells.Length);
        Assert.Equal(first.Cells.ToArray(), second.Cells.ToArray());
        Assert.Equal(elevations[5 * definition.Bounds.Width + 7], first.GetCell(7, 5).TerrainElevationMetres);
        Assert.Equal(SpatialBiomeIds.TemperateForest,
            SpatialBiomeClassifier.Classify(definition.Profile, 12d, 850d));
        Assert.Equal(SpatialBiomeIds.Meadow,
            SpatialBiomeClassifier.Classify(definition.Profile, 30.01d, 4_000d));
        Assert.Equal(SpatialBiomeIds.Meadow,
            SpatialBiomeClassifier.Classify(definition.Profile, 10d, 849.99d));
    }

    [Fact]
    public void Terrain_elevation_changes_temperature_and_rising_terrain_changes_precipitation() {
        var definition = Definition(seed: 9021, width: 16, height: 9);
        var flat = Enumerable.Repeat(0d, definition.ExpectedCellCount).ToArray();
        var mountain = flat.ToArray();
        for (var z = 0; z < definition.Bounds.Height; z++) mountain[z * definition.Bounds.Width + 8] = 1_000d;
        var generator = new EnvironmentalAtlasGenerator();

        var baseline = generator.Generate(definition, flat);
        var raised = generator.Generate(definition, mountain);

        Assert.True(raised.GetCell(8, 4).TemperatureCelsius < baseline.GetCell(8, 4).TemperatureCelsius);
        Assert.True(raised.GetCell(8, 4).AtmosphericPressurePascals <
                    baseline.GetCell(8, 4).AtmosphericPressurePascals);
        Assert.True(raised.GetCell(8, 4).AnnualPrecipitationMillimetres >
                    baseline.GetCell(8, 4).AnnualPrecipitationMillimetres);
        Assert.True(raised.GetCell(9, 4).Humidity < baseline.GetCell(9, 4).Humidity);
    }

    [Fact]
    public void Oceans_supply_more_moisture_and_circulation_has_a_west_to_east_component() {
        var definition = Definition(seed: 771, width: 10, height: 8);
        var ocean = Enumerable.Repeat(-2d, definition.ExpectedCellCount).ToArray();
        var land = Enumerable.Repeat(5d, definition.ExpectedCellCount).ToArray();
        var generator = new EnvironmentalAtlasGenerator();

        var oceanAtlas = generator.Generate(definition, ocean);
        var landAtlas = generator.Generate(definition, land);

        Assert.True(oceanAtlas.GetCell(4, 4).Humidity > landAtlas.GetCell(4, 4).Humidity);
        Assert.True(oceanAtlas.GetCell(4, 4).AnnualPrecipitationMillimetres >
                    landAtlas.GetCell(4, 4).AnnualPrecipitationMillimetres);
        Assert.All(oceanAtlas.Cells.ToArray(), cell => {
            Assert.True(cell.CirculationX > 0d);
            Assert.InRange(cell.CirculationX, 0.65d, 1d);
            Assert.InRange(cell.CirculationZ, -0.3d, 0.3d);
            Assert.InRange(cell.Humidity, 0d, 1d);
            Assert.InRange(cell.SurfaceMoisture, 0d, 1d);
            Assert.InRange(cell.AnnualPrecipitationMillimetres, 0d, 4_000d);
            Assert.InRange(cell.AtmosphericPressurePascals, 0d, 200_000d);
        });
    }

    [Fact]
    public void Different_seeds_and_invalid_inputs_are_distinguished() {
        var firstDefinition = Definition(seed: 1, width: 8, height: 8);
        var secondDefinition = Definition(seed: 2, width: 8, height: 8);
        var elevations = new double[firstDefinition.ExpectedCellCount];
        var generator = new EnvironmentalAtlasGenerator();

        var first = generator.Generate(firstDefinition, elevations);
        var second = generator.Generate(secondDefinition, elevations);

        Assert.NotEqual(first.Cells.ToArray(), second.Cells.ToArray());
        Assert.Throws<ArgumentException>(() => generator.Generate(firstDefinition, new double[1]));
        elevations[7] = double.NaN;
        Assert.Throws<ArgumentOutOfRangeException>(() => generator.Generate(firstDefinition, elevations));
        Assert.Throws<ArgumentOutOfRangeException>(() => new EnvironmentalAtlasBounds(0, 0, 2, 2, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new EnvironmentalAtlasDefinition(
            1, 1, "goxlap", "profile", EnvironmentalAtlasProfile.TemperateAtlasV1,
            new EnvironmentalAtlasBounds(0, 0, 2, 2, 512), double.PositiveInfinity));
    }

    [Fact]
    public void Climate_generation_does_not_retain_or_mutate_the_supplied_raster() {
        var definition = Definition(seed: 87, width: 7, height: 5);
        var elevations = Enumerable.Repeat(12d, definition.ExpectedCellCount).ToArray();
        var atlas = new EnvironmentalAtlasGenerator().Generate(definition, elevations);
        var retained = atlas.GetCell(2, 3).TerrainElevationMetres;

        elevations[3 * definition.Bounds.Width + 2] = -100d;

        Assert.Equal(12d, retained);
        Assert.Equal(12d, atlas.GetCell(2, 3).TerrainElevationMetres);
    }

    [Fact]
    public void Signed_noise_coordinates_are_order_independent_and_continuous_across_zero() {
        const long seed = -9_217;
        const ulong salt = 0xD1B54A32D192ED03UL;
        (double X, double Z)[] coordinates = [
            (-16_384.25d, -8_192.75d),
            (-0.001d, 0.001d),
            (0.001d, -0.001d),
            (24_576.5d, -32_768.125d),
        ];

        var forward = coordinates
            .Select(point => EnvironmentalValueNoise.Sample(seed, salt, point.X, point.Z, 8_192d))
            .ToArray();
        var reverse = coordinates.Reverse()
            .Select(point => EnvironmentalValueNoise.Sample(seed, salt, point.X, point.Z, 8_192d))
            .Reverse()
            .ToArray();
        var belowZero = EnvironmentalValueNoise.Sample(seed, salt, -0.000001d, -0.000001d, 8_192d);
        var aboveZero = EnvironmentalValueNoise.Sample(seed, salt, 0.000001d, 0.000001d, 8_192d);

        Assert.Equal(forward, reverse);
        Assert.InRange(Math.Abs(belowZero - aboveZero), 0d, 1e-12d);
        Assert.All(forward, value => Assert.InRange(value, 0d, Math.BitDecrement(1d)));
    }

    private static EnvironmentalAtlasDefinition Definition(long seed, int width, int height) => new(
        seed,
        worldGenerationVersion: 20,
        terrainProviderIdentity: "goxlap",
        terrainProfileIdentity: "goxlap-test-profile",
        EnvironmentalAtlasProfile.TemperateAtlasV1,
        new EnvironmentalAtlasBounds(-4_096d, -2_048d, width, height, 512d),
        oceanElevationMetres: -1d);
}

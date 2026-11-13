using Weltschmerz.Spatial.Biomes;
using Weltschmerz.Spatial.Climate;
using System.Threading;

namespace Weltschmerz.Spatial.Atlas;

/// <summary>Generates one complete immutable climate and biome dataset from caller-supplied terrain elevations.</summary>
public sealed class EnvironmentalAtlasGenerator {
    public EnvironmentalAtlas Generate(
        EnvironmentalAtlasDefinition definition,
        ReadOnlyMemory<double> terrainElevations,
        CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(definition);
        cancellationToken.ThrowIfCancellationRequested();
        if (terrainElevations.Length != definition.ExpectedCellCount)
            throw new ArgumentException("Terrain elevation raster must contain exactly one value per atlas cell.", nameof(terrainElevations));
        var elevations = terrainElevations.Span;
        for (var index = 0; index < elevations.Length; index++) {
            if (!double.IsFinite(elevations[index]))
                throw new ArgumentOutOfRangeException(nameof(terrainElevations), $"Terrain elevation at row-major index {index} is not finite.");
        }

        var temperatures = TemperatureFieldGenerator.Generate(definition, elevations, cancellationToken);
        var (pressure, circulationX, circulationZ) = CirculationFieldGenerator.Generate(
            definition, elevations, temperatures, cancellationToken);
        var (humidity, surfaceMoisture, precipitation) = MoistureFieldGenerator.Generate(
            definition, elevations, circulationX, circulationZ, cancellationToken);
        var cells = new EnvironmentalAtlasCell[elevations.Length];
        for (var index = 0; index < cells.Length; index++) {
            if ((index & 0x3ff) == 0) cancellationToken.ThrowIfCancellationRequested();
            var biome = SpatialBiomeClassifier.Classify(definition.Profile, temperatures[index], precipitation[index]);
            cells[index] = new(elevations[index], temperatures[index], pressure[index],
                circulationX[index], circulationZ[index], humidity[index], surfaceMoisture[index],
                precipitation[index], biome);
        }
        cancellationToken.ThrowIfCancellationRequested();
        return new EnvironmentalAtlas(definition, cells);
    }
}

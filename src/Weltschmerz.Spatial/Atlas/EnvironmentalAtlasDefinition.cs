namespace Weltschmerz.Spatial.Atlas;

/// <summary>Immutable inputs identifying one complete terrain-informed atlas generation.</summary>
public sealed class EnvironmentalAtlasDefinition {
    public long WorldSeed { get; }
    public int WorldGenerationVersion { get; }
    public string TerrainProviderIdentity { get; }
    public string TerrainProfileIdentity { get; }
    public EnvironmentalAtlasProfile Profile { get; }
    public EnvironmentalAtlasBounds Bounds { get; }
    public double OceanElevationMetres { get; }
    public int ExpectedCellCount => checked(Bounds.Width * Bounds.Height);

    public EnvironmentalAtlasDefinition(
        long worldSeed,
        int worldGenerationVersion,
        string terrainProviderIdentity,
        string terrainProfileIdentity,
        EnvironmentalAtlasProfile profile,
        EnvironmentalAtlasBounds bounds,
        double oceanElevationMetres) {
        if (worldGenerationVersion <= 0) throw new ArgumentOutOfRangeException(nameof(worldGenerationVersion));
        ArgumentException.ThrowIfNullOrWhiteSpace(terrainProviderIdentity);
        ArgumentException.ThrowIfNullOrWhiteSpace(terrainProfileIdentity);
        ArgumentNullException.ThrowIfNull(profile);
        if (!double.IsFinite(oceanElevationMetres)) throw new ArgumentOutOfRangeException(nameof(oceanElevationMetres));
        _ = checked(bounds.Width * bounds.Height);

        WorldSeed = worldSeed;
        WorldGenerationVersion = worldGenerationVersion;
        TerrainProviderIdentity = terrainProviderIdentity;
        TerrainProfileIdentity = terrainProfileIdentity;
        Profile = profile;
        Bounds = bounds;
        OceanElevationMetres = oceanElevationMetres;
    }
}

namespace Weltschmerz.Spatial.Biomes;

/// <summary>A stable non-zero identity for a baseline land biome.</summary>
public readonly record struct SpatialBiomeId {
    public ushort Value { get; }

    public SpatialBiomeId(ushort value) {
        if (value == 0) throw new ArgumentOutOfRangeException(nameof(value), "Biome id 0 is reserved.");
        Value = value;
    }
}

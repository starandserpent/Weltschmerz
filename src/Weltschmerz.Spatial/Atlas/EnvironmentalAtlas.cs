namespace Weltschmerz.Spatial.Atlas;

/// <summary>Immutable complete environmental dataset produced before a world becomes playable.</summary>
public sealed class EnvironmentalAtlas {
    private readonly EnvironmentalAtlasCell[] _cells;

    internal EnvironmentalAtlas(EnvironmentalAtlasDefinition definition, EnvironmentalAtlasCell[] cells) {
        Definition = definition ?? throw new ArgumentNullException(nameof(definition));
        ArgumentNullException.ThrowIfNull(cells);
        if (cells.Length != definition.ExpectedCellCount)
            throw new ArgumentException("Environmental cell count differs from the complete atlas definition.", nameof(cells));
        _cells = cells;
    }

    public EnvironmentalAtlasDefinition Definition { get; }
    public ReadOnlyMemory<EnvironmentalAtlasCell> Cells => _cells;

    public EnvironmentalAtlasCell GetCell(int x, int z) {
        if ((uint)x >= (uint)Definition.Bounds.Width) throw new ArgumentOutOfRangeException(nameof(x));
        if ((uint)z >= (uint)Definition.Bounds.Height) throw new ArgumentOutOfRangeException(nameof(z));
        return _cells[checked(z * Definition.Bounds.Width + x)];
    }
}

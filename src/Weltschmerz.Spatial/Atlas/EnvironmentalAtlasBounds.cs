namespace Weltschmerz.Spatial.Atlas;

/// <summary>Finite half-open geographical coverage sampled at cell centers.</summary>
public readonly record struct EnvironmentalAtlasBounds {
    public double OriginXMetres { get; }
    public double OriginZMetres { get; }
    public int Width { get; }
    public int Height { get; }
    public double CellSpacingMetres { get; }
    public double MaximumXExclusiveMetres => OriginXMetres + Width * CellSpacingMetres;
    public double MaximumZExclusiveMetres => OriginZMetres + Height * CellSpacingMetres;

    public EnvironmentalAtlasBounds(double originXMetres, double originZMetres, int width, int height, double cellSpacingMetres) {
        if (!double.IsFinite(originXMetres)) throw new ArgumentOutOfRangeException(nameof(originXMetres));
        if (!double.IsFinite(originZMetres)) throw new ArgumentOutOfRangeException(nameof(originZMetres));
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));
        if (!double.IsFinite(cellSpacingMetres) || cellSpacingMetres <= 0d)
            throw new ArgumentOutOfRangeException(nameof(cellSpacingMetres));
        if (!double.IsFinite(originXMetres + width * cellSpacingMetres) ||
            !double.IsFinite(originZMetres + height * cellSpacingMetres))
            throw new ArgumentOutOfRangeException(nameof(cellSpacingMetres), "Atlas bounds exceed the finite geographical coordinate range.");

        OriginXMetres = originXMetres;
        OriginZMetres = originZMetres;
        Width = width;
        Height = height;
        CellSpacingMetres = cellSpacingMetres;
    }

    public (double X, double Z) CellCenter(int x, int z) {
        if ((uint)x >= (uint)Width) throw new ArgumentOutOfRangeException(nameof(x));
        if ((uint)z >= (uint)Height) throw new ArgumentOutOfRangeException(nameof(z));
        return (OriginXMetres + (x + 0.5d) * CellSpacingMetres,
            OriginZMetres + (z + 0.5d) * CellSpacingMetres);
    }

    public bool Contains(double worldXMetres, double worldZMetres) =>
        double.IsFinite(worldXMetres) && double.IsFinite(worldZMetres) &&
        worldXMetres >= OriginXMetres && worldXMetres < MaximumXExclusiveMetres &&
        worldZMetres >= OriginZMetres && worldZMetres < MaximumZExclusiveMetres;
}

using Weltschmerz.Spatial.Biomes;

namespace Weltschmerz.Spatial.Atlas;

/// <summary>Complete static environmental sample for one atlas cell.</summary>
public readonly record struct EnvironmentalAtlasCell(
    double TerrainElevationMetres,
    double TemperatureCelsius,
    double AtmosphericPressurePascals,
    double CirculationX,
    double CirculationZ,
    double Humidity,
    double SurfaceMoisture,
    double AnnualPrecipitationMillimetres,
    SpatialBiomeId BiomeId);

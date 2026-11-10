namespace Weltschmerz.Spatial.Atlas;

/// <summary>Immutable, versioned parameters for the first temperate environmental atlas.</summary>
public sealed class EnvironmentalAtlasProfile {
    public static EnvironmentalAtlasProfile TemperateAtlasV1 { get; } = new();

    public string Identity => "temperate-atlas-v1";
    public int AlgorithmVersion => 1;
    public double NominalSeaLevelTemperatureCelsius => 18d;
    public double TemperatureLapseRateCelsiusPerMetre => 0.0065d;
    public double MaximumNorthSouthTemperatureAdjustmentCelsius => 8d;
    public double NorthSouthAdjustmentDistanceMetres => 65_536d;
    public double TemperatureNoiseAmplitudeCelsius => 6d;
    public double TemperatureNoiseSpacingMetres => 8_192d;
    public double WindVariationSpacingMetres => 8_192d;
    public double PrecipitationNoiseSpacingMetres => 4_096d;
    public double ReferenceAtmosphericPressurePascals => 101_325d;
    public double PressureScaleHeightMetres => 8_434d;
    public double MaximumAnnualPrecipitationMillimetres => 4_000d;
    public double ForestMinimumTemperatureCelsius => -5d;
    public double ForestMaximumTemperatureCelsius => 30d;
    public double ForestPrecipitationThresholdMillimetres => 850d;
    public double WesternBoundaryHumidity => 0.45d;
    public double OceanEvaporationContribution => 0.24d;
    public double LandMoistureContribution => 0.035d;
    public double UpwindHumidityRetention => 0.82d;
    public double PrecipitationMoistureLoss => 0.62d;
    public double BaselinePrecipitationMillimetres => 80d;
    public double NoisePrecipitationVariationMillimetres => 700d;
    public double HumidityPrecipitationContributionMillimetres => 1_250d;
    public double OrographicPrecipitationPerMetreRiseMillimetres => 1.2d;
    public double MaximumOrographicPrecipitationMillimetres => 1_500d;

    private EnvironmentalAtlasProfile() { }
}

using System.Numerics;

/// <summary>
/// Main class of the Weltschmerz generator
/// Initializes all generators and provides all their functions in one class
/// </summary>
public class Weltschmerz {
    /// <summary>
    /// Main class of the Weltschmerz generator
    /// Initializes all generators and provides all their functions in one class
    /// </summary>
    public ElevationGenerator ElevationGenerator { get; set; }
    public CirculationGenerator CirculationGenerator { get; set; }
    public PrecipitationGenerator PrecipitationGenerator { get; set; }
    public TemperatureGenerator TemperatureGenerator { get; set; }
    private Config config;
    public Weltschmerz () : this (ConfigManager.GetConfig ()) {}

    public Weltschmerz (Config config) {
        this.config = config;
        this.ElevationGenerator = new Elevation (this, config);
        this.TemperatureGenerator = new Temperature (this, config);
        this.CirculationGenerator = new Circulation (this, config);
        this.PrecipitationGenerator = new Precipitation (this, config);
    }

    public double GetTemperature (int posX, int posY) {
        double elevation = ElevationGenerator.GetNoise (posX, posY);
        return GetTemperature (posY, elevation);
    }

    public double GetTemperature (int posY, double elevation) {
        return TemperatureGenerator.GetTemperature (posY, elevation);
    }

    public double GetElevation (int posX, int posY) {
        return ElevationGenerator.GetNoise (posX, posY);
    }

    public double GetPrecipitation (int posX, int posY, double elevation, double temperature) {
        return PrecipitationGenerator.GetPrecipitation (posX, posY, elevation, temperature);
    }

    public double GetPrecipitation (int posX, int posY, double elevation) {
        double temperature = TemperatureGenerator.GetTemperature (posX, elevation);
        return PrecipitationGenerator.GetPrecipitation (posX, posY, elevation, temperature);
    }

    public double GetPrecipitation (int posX, int posY) {
        double elevation = ElevationGenerator.GetNoise (posX, posY);
        double temperature = TemperatureGenerator.GetTemperature (posX, elevation);
        return GetPrecipitation (posX, posY, elevation, temperature);
    }

    public Config GetConfig () {
        return config;
    }
}
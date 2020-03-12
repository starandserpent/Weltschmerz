using System.Numerics;
//Class for handling all environmental variables together
public class Weltschmerz {
    public NoiseGenerator NoiseGenerator { get; set; }
    public CirculationGenerator CirculationGenerator { get; set; }
    public PrecipitationGenerator PrecipitationGenerator { get; set; }
    public TemperatureGenerator TemperatureGenerator { get; set; }
    private Config config;
    public Weltschmerz () : this (ConfigManager.GetConfig ()) { }

    public Weltschmerz (Config config) {
        this.config = config;
        this.NoiseGenerator = new Noise (config);
        this.TemperatureGenerator = new Temperature (config);
        this.CirculationGenerator = new Circulation (config, this);
        this.PrecipitationGenerator = new Precipitation (config, this);
    }

    public double GetTemperature (int posX, int posY) {
        double elevation = NoiseGenerator.GetNoise (posX, posY);
        return GetTemperature (posY, elevation);
    }

    public double GetTemperature (int posY, double elevation) {
        return TemperatureGenerator.GetTemperature (posY, elevation);
    }

    public double GetElevation (int posX, int posY) {
        return NoiseGenerator.GetNoise (posX, posY);
    }

    public double GetPrecipitation (int posX, int posY, double elevation, double temperature) {
        return PrecipitationGenerator.GetPrecipitation (posX, posY, elevation, temperature);
    }

    public double GetPrecipitation (int posX, int posY, double elevation) {
        double temperature = TemperatureGenerator.GetTemperature (posX, elevation);
        return PrecipitationGenerator.GetPrecipitation (posX, posY, elevation, temperature);
    }

    public double GetPrecipitation (int posX, int posY) {
        double elevation = NoiseGenerator.GetNoise (posX, posY);
        double temperature = TemperatureGenerator.GetTemperature (posX, elevation);
        return GetPrecipitation (posX, posY, elevation, temperature);
    }

    public Config GetConfig () {
        return config;
    }
}
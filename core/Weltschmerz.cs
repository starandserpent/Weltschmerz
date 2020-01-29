using System.Numerics;
//Class for handling all environmental variables together
public class Weltschmerz : IConfigurable
{
    NoiseGenerator NoiseGenerator{get; set;}
    CirculationGenerator CirculationGenerator{get; set;}
    PrecipitationGenerator PrecipitationGenerator{get; set;}
    TemperatureGenerator TemperatureGenerator{get; set;}
    private Config config;
    public Weltschmerz() : this (ConfigManager.GetConfig()){}

    public Weltschmerz(Config config)
    {
        this.config = config;
        this.NoiseGenerator = new Noise(config);
        this.TemperatureGenerator = new Temperature(config);
        this.CirculationGenerator = new Circulation(config, NoiseGenerator, TemperatureGenerator);
        this.PrecipitationGenerator = new Precipitation(config, NoiseGenerator, TemperatureGenerator);
    }

    public void Update(){
        Configure(config);
    }

    public void Configure(Config config){
        NoiseGenerator.Configure(config);
        PrecipitationGenerator.Configure(config);
        TemperatureGenerator.Configure(config);
        CirculationGenerator.Configure(config);
    }

    public double GetTemperature(int posX, int posY){
        double elevation = NoiseGenerator.GetNoise(posX, posY);
        return GetTemperature(posY, elevation);
    }

    public double GetTemperature(int posY, double elevation){
        return TemperatureGenerator.GetTemperature(posY, elevation);
    }

    public double GetElevation(int posX, int posY)
    {
        return NoiseGenerator.GetNoise(posX, posY);
    }

    public double GetPrecipitation(int posX, int posY, double elevation, double temperature, Vector2 wind){
        return PrecipitationGenerator.GetPrecipitation(posX, posY, elevation, temperature, wind);
    }

    public double GetPrecipitation(int posX, int posY, double elevation, double temperature){
        Vector2 wind = CirculationGenerator.GetAirFlow(posX, posY);
        return PrecipitationGenerator.GetPrecipitation(posX, posY, elevation, temperature, wind);
    }

    public double GetPrecipitation(int posX, int posY, double elevation){
        double temperature = TemperatureGenerator.GetTemperature(posX, elevation);
        Vector2 wind = CirculationGenerator.GetAirFlow(posX, posY);
        return PrecipitationGenerator.GetPrecipitation(posX, posY, elevation, temperature, wind);
    }

    public double GetPrecipitation(int posX, int posY){
        double elevation = NoiseGenerator.GetNoise(posX, posY);
        double temperature = TemperatureGenerator.GetTemperature(posX, elevation);
        Vector2 wind = CirculationGenerator.GetAirFlow(posX, posY);
        return GetPrecipitation(posX, posY, elevation, temperature, wind);
    }

    public Vector2 GetAirFlow(int posX, int posY){
        double elevation = NoiseGenerator.GetNoise(posX, posY);
        double temperature = TemperatureGenerator.GetTemperature(posX, elevation);
        return CirculationGenerator.GetAirFlow(posX, posY);
    }
    public Config GetConfig(){
        return config;
    }
}
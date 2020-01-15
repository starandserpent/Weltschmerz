using System.Numerics;
//Class for handling all environmental variables together
public class Weltschmerz : IConfigurable
{
    private volatile Noise noise; 
    private Circulation circulation;
    private Precipitation precipitation;
    private Temperature equator;
    private Config config;
    public Weltschmerz() : this (ConfigManager.GetConfig()){}

    public Weltschmerz(Config config)
    {
        this.config = config;
        this.noise = new Noise(config);
        this.equator = new Temperature(config);
        this.circulation = new Circulation(config, noise, equator);
        this.precipitation = new Precipitation(config, noise, equator);
    }

    public void Configure(Config config){
        noise.Configure(config);
        precipitation.Configure(config);
        equator.Configure(config);
        circulation.Configure(config);
    }

    public double GetTemperature(int posX, int posY){
        double elevation = noise.GetNoise(posX, posY);
        return equator.GetTemperature(posY, elevation);
    }

    public double GetTemperature(int posY, double elevation){
        return equator.GetTemperature(posY, elevation);
    }

    public double GetElevation(int posX, int posY)
    {
        return noise.GetNoise(posX, posY);
    }

    public double GetPrecipitation(int posX, int posY){
        double elevation = noise.GetNoise(posX, posY);
        double temperature = equator.GetTemperature(posX, elevation);
        Vector2 wind = circulation.GetAirFlow(posX, posY);
        return precipitation.GetPrecipitation(posX, posY, elevation, temperature, wind);
    }

    public int GetMaxElevation(){
        return noise.GetMaxElevation();
    }
}
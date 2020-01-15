//Class for handling all environmental variables together
public class Weltschmerz : IConfigurable
{
    private volatile Noise noise; 
    private Circulation circulation;
    private Precipitation precipitation;
    private Temperature temperature;
    private Config config;
    public Weltschmerz() : this (ConfigManager.GetConfig()){}

    public Weltschmerz(Config config)
    {
        this.config = config;
        this.noise = new Noise(config);
        this.temperature = new Temperature(config);
        this.circulation = new Circulation(config, noise, temperature);
        this.precipitation = new Precipitation(config);
    }

    public void Configure(Config config){
        noise.Configure(config);
        precipitation.Configure(config);
        temperature.Configure(config);
        circulation.Configure(config);
    }

    public double GetTemperature(int posX, int posY){
        double elevation = noise.getNoise(posX, posY);
        return temperature.GetTemperature(posY, elevation);
    }

    public double GetTemperature(int posY, double elevation){
        return temperature.GetTemperature(posY, elevation);
    }

    public double GetElevation(int posX, int posY)
    {
        return noise.getNoise(posX, posY);
    }

    public double GetPrecipitation(int posX, int posY){
        return 0.0;
    }

    public int GetMaxElevation(){
        return noise.GetMaxElevation();
    }
}
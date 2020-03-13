
/// <summary>
/// Main class of the Weltschmerz generator
/// Initializes all generators and provides all their functions in one class
/// </summary>
public class Weltschmerz {
    /// <summary>
    /// Current elevation generator used in Weltschmerz and other generators
    /// You can replace this abstract class by your own implementation
    /// </summary>
    public ElevationGenerator ElevationGenerator { get; set; }

    /// <summary>
    /// Current circulation generator used in Weltschmerz and other generators
    /// You can replace this abstract class by your own implementation
    /// </summary>
    public CirculationGenerator CirculationGenerator { get; set; }

    /// <summary>
    /// Current precipitation generator used in Weltschmerz and other generators
    /// You can replace this abstract class by your own implementation
    /// </summary>
    public PrecipitationGenerator PrecipitationGenerator { get; set; }

    /// <summary>
    /// Current temperature generator used in Weltschmerz and other generators
    /// You can replace this abstract class by your own implementation
    /// </summary>
    public TemperatureGenerator TemperatureGenerator { get; set; }

    //Current config class
    public Config config {get; private set;}

    /// <summary>
    /// Initializes all default Weltschmerz generators and sets default config from default config fiel
    /// </summary>
    public Weltschmerz () : this (ConfigManager.GetConfig ()) { }

    /// <summary>
    /// Initializes all default Weltschmerz generators and sets specific config
    /// </summary>
    public Weltschmerz (Config config) {
        this.config = config;

        //Initializes all default Weltschmerz generators
        this.ElevationGenerator = new Elevation (this, config);
        this.TemperatureGenerator = new Temperature (this, config);
        this.CirculationGenerator = new Circulation (this, config);
        this.PrecipitationGenerator = new Precipitation (this, config);
    }

    /// <summary>
    /// Gets temperature on current position
    /// Use <see cref="TemperatureGenerator"/> for more detailed access
    /// </summary>
    public double GetTemperature (int posX, int posY) {
        double elevation = ElevationGenerator.GetElevation (posX, posY);
        return TemperatureGenerator.GetTemperature (posY, elevation);
    }

    /// <summary>
    /// Gets elevation on current position
    /// Use <see cref="ElevationGenerator"/> for more detailed access
    /// </summary>
    public double GetElevation (int posX, int posY) {
        return ElevationGenerator.GetElevation (posX, posY);
    }

    /// <summary>
    /// Gets moisture on current position
    /// Use <see cref="PrecipitationGenerator"/> for more detailed access
    /// </summary>
    public double GetMoisture (int posY) {
        return PrecipitationGenerator.GetMoisture (posY);
    }

    /// <summary>
    /// Gets wind delta on current position
    /// Use <see cref="CirculationGenerator"/> for more detailed access
    /// </summary>
    public double GetWindDelta (int posX, int posY) {
        double elevation = ElevationGenerator.GetElevation (posX, posY);
        double airpressure = CirculationGenerator.GetAirPressure(posX, posY, elevation);
        return CirculationGenerator.GetWindDelta (posX, posY, airpressure, ElevationGenerator.IsLand(elevation), elevation);
    }

    /// <summary>
    /// Gets air pressure on current position
    /// Use <see cref="CirculationGenerator"/> for more detailed access
    /// </summary>
    public double GetAirPressure (int posX, int posY) {
        double elevation = ElevationGenerator.GetElevation (posX, posY);
        return CirculationGenerator.GetAirPressure(posX, posY, elevation);
    }


    /// <summary>
    /// Gets humidity on current position
    /// Use <see cref="PrecipitationGenerator"/> for more detailed access
    /// </summary>
    public double GetHumidity (int posX, int posY) {
        double elevation = ElevationGenerator.GetElevation (posX, posY);
        double temperature = TemperatureGenerator.GetTemperature (posX, elevation);
        return PrecipitationGenerator.GetHumidity (posX, posY, elevation);
    }

    /// <summary>
    /// Gets precipitation on current position
    /// Use <see cref="PrecipitationGenerator"/> for more detailed access
    /// </summary>
    public double GetPrecipitation (int posX, int posY) {
        double elevation = ElevationGenerator.GetElevation (posX, posY);
        double temperature = TemperatureGenerator.GetTemperature (posX, elevation);
        return PrecipitationGenerator.GetPrecipitation (posX, posY, elevation, temperature);
    }

    /// <summary>
    /// Calls <see cref="Generator.ChangeConfig(Config)"/> function from <see cref="Generator"/>
    /// </summary>
    public void ChangeConfig (Config config) {
        this.ElevationGenerator.ChangeConfig (config);
        this.TemperatureGenerator.ChangeConfig (config);
        this.PrecipitationGenerator.ChangeConfig (config);
        this.CirculationGenerator.ChangeConfig (config);
    }

    /// <summary>
    /// Calls <see cref="Generator.Update()"/> function from <see cref="Generator"/>
    /// </summary>
    public void Update () {
        this.ElevationGenerator.Update ();
        this.TemperatureGenerator.Update ();
        this.PrecipitationGenerator.Update ();
        this.CirculationGenerator.Update ();
    }
}
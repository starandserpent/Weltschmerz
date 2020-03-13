/// <summary>
/// Class used for simulating everything related to precipitation
/// (Moisture, Humidity, Precipitation)
/// </summary>
public abstract class PrecipitationGenerator : Generator {

    public PrecipitationGenerator (Weltschmerz weltschmerz, Config config) : base (weltschmerz, config) { }

    /// <return>
    /// Returns estimated moisture based on latitude
    /// </return>
    public abstract double GetMoisture (int posY);

    /// <return>
    /// Returns humidity calculated from moisture and wind on specific position
    /// </return>
    public abstract double GetHumidity (int posX, int posY, double elevation);

    /// <return>
    /// Moisture canged by terrain (difference between land and ocean) on specific position
    /// </return>
    public abstract double GetEvapotranspiration (int posY, bool isLand);

    /// <return>
    /// Simulated precipitation based on moisture and humidity on specific position
    /// </return>
    public abstract double GetPrecipitation (int posX, int posY, double elevation, double temperature);
}
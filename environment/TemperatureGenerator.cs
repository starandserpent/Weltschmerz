
/// <summary>
/// Class used for simulating temperature and equator
/// </summary>
public abstract class TemperatureGenerator : Generator {

    /// <summary>
    /// Equator position in the world
    /// </summary>
    public double EquatorPosition { get; set; }

    /// <summary>
    /// Temperature decrease per distance
    /// </summary>
    public double LapseRate { get; protected set; }

    public TemperatureGenerator (Weltschmerz weltschmerz, Config config) : base(weltschmerz, config) {}

    /// <summary>
    /// Basic temperature calculated from distance to equator
    /// </summary>
    public abstract double GetTemperatureAtSeaLevel (int posY);

    /// <summary>
    /// Basic temperature influenced by lapse rate and elevation
    /// </summary>
    public abstract double GetTemperature (int posY, double elevation);

    /// <summary>
    /// Distance to equator from posY 
    /// </summary>
    public abstract double GetEquatorDistance (int posY);
}
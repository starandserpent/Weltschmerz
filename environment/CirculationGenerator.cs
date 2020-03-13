/// <summary>
/// Class used for simulating air pressure and wind speed
/// </summary>
public abstract class CirculationGenerator : Generator {

    public CirculationGenerator (Weltschmerz weltschmerz, Config config) : base (weltschmerz, config) { }

    /// <return>
    /// Returns wind delta on specified position
    /// </return>
    public abstract double GetWindDelta (int posX, int posY, double pressure, bool isLand, double newEslevation);

    /// <return>
    /// Returns air pressure on specified position
    /// </return>
    public abstract double GetAirPressure (int posX, int posY, double elevation);
}
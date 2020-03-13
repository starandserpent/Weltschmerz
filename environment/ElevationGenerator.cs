/// <summary>
/// Class simulating everything related to terrain and its elevation
/// This class generates random terrain based on seed
/// </summary>
public abstract class ElevationGenerator : Generator {
    public ElevationGenerator (Weltschmerz weltschmerz, Config config) : base(weltschmerz, config) {}

    /// <return>
    /// Returns terrain elevation on current position
    /// </return>
    public abstract double GetElevation (int posX, int posY);
    
    /// <return>
    /// Returns True if current elevation is land
    /// Returns False if elevation is water/ocean/sea
    /// </return>
    public abstract bool IsLand (double elevation);
}
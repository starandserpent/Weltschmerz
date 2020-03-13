public abstract class ElevationGenerator : Generator {
    public ElevationGenerator (Weltschmerz weltschmerz, Config config) : base(weltschmerz, config) {}

    public abstract double GetNoise (int posX, int posY);

    
    public abstract bool IsLand (double elevation);
}
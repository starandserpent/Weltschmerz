using System.Numerics;
public abstract class CirculationGenerator : Generator {

    public CirculationGenerator (Weltschmerz weltschmerz, Config config) : base(weltschmerz, config) {}

    public abstract double GetAirFlow (int posX, int posY, double pressure, double elevation);

    public abstract double GetAirPressure (int posX, int posY, double elevation);
}
using System.Numerics;
public abstract class CirculationGenerator {
    protected Config config;

    public CirculationGenerator (Config config) {
        this.config = config;
    }

    public abstract double GetAirFlow (int posX, int posY, double pressure, double elevation);

    public abstract double GetAirPressure (int posX, int posY, double elevation);
}
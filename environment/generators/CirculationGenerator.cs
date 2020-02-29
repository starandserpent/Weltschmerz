using System.Numerics;
public abstract class CirculationGenerator
{
    protected Config config;

    public CirculationGenerator(Config config){
        this.config = config;
    }

    public abstract Vector2 GetAirFlow(int posX, int posY, double pressure, double baseElevation, double elevation);

    public abstract double CalculateDensity(int posX, int posY);
}
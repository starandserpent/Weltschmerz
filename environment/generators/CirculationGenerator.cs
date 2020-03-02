using System.Numerics;
public abstract class CirculationGenerator
{
    protected Config config;

    public CirculationGenerator(Config config){
        this.config = config;
    }

    public abstract Vector2 GetAirFlow(int posX, int posY, double pressure);

    public abstract double GetAirPressure(int posX, int posY);
}
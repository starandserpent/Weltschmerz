using System.Numerics;
public abstract class PrecipitationGenerator
{

    protected Config config;

    public PrecipitationGenerator(Config config){
        this.config = config;
    }

    public abstract double GetPrecipitation(int posX, int posY, double elevation, double temperature, Vector2 wind);
}
using System.Numerics;
public abstract class PrecipitationGenerator
{

    protected Config config;

    public PrecipitationGenerator(Config config){
        this.config = config;
    }
    public abstract double GetMoisture(int posY);

    public abstract double GetHumidity(int posX, int posY, double elevation);
    public abstract double GetEvapotranspiration(int posY, bool isLand);

    public abstract double GetPrecipitation(int posX, int posY, double elevation, double temperature);
}
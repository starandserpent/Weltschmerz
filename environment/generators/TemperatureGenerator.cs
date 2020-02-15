public abstract class TemperatureGenerator
{
    public double EquatorPosition{get; set;}

    protected Config config;

    public TemperatureGenerator(Config config){
        this.config = config;
    }

    public abstract double GetTemperature(int posY, double elevation);
    public abstract double GetEquatorDistance(int posY);
}
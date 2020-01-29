public abstract class TemperatureGenerator : IConfigurable
{
    public double EquatorPosition{get; set;}
    public abstract double GetTemperature(int posY, double elevation);
    public abstract void Configure(Config config);
    public abstract double GetEquatorDistance(int posY);
}
using System.Numerics;
public abstract class PrecipitationGenerator : IConfigurable
{
    public abstract double GetPrecipitation(int posX, int posY, double elevation, double temperature, Vector2 wind);
    public abstract void Configure(Config config);   
}
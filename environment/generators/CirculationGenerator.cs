using System.Numerics;
public abstract class CirculationGenerator : IConfigurable
{
    public abstract Vector2 GetAirFlow(int posX, int posY);
    public abstract void Configure(Config config);
}
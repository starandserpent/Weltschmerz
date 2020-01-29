public abstract class NoiseGenerator : IConfigurable
{
    public abstract double GetNoise(int posX, int posY);
    public abstract void Configure(Config config);
}
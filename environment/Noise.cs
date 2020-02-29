using System;
public class Noise : NoiseGenerator
{
    private volatile FastNoise noise;
    private double multiplier;
    private double tau;
    public Noise(Config config) : base(config){
        noise = new FastNoise(config.map.seed);
        tau = 2 * Math.PI;
        multiplier = 1.0 / tau;

        noise.SetFrequency((float)config.noise.frequency);
        noise.SetFractalOctaves(10);
    }

    public override double GetNoise(int x, int y)
    {
            float s = x / (float) config.map.longitude;
            float t = y / (float) config.map.latitude;
            double nx = Math.Cos(s * tau) * multiplier;
            double ny = Math.Cos(t * tau) * multiplier;
            double nz = Math.Sin(s * tau) * multiplier;
            double nw = Math.Sin(t * tau) * multiplier;

            double n1 = noise.GetPerlinFractal((float) nx,(float) ny);
            double n2 = noise.GetPerlinFractal((float) nz,(float) nw);

            return Math.Min(Math.Max(noise.GetPerlinFractal((float)n1, (float)n2) * config.noise.max_elevation, config.noise.min_elevation), config.noise.max_elevation);
    }
}
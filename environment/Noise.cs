using System;
public class Noise : NoiseGenerator
{
    private volatile FastNoise noise;
    private double multiplier;
    private double tau;
    private int averageElevation;
    public Noise(Config config) : base(config){
        noise = new FastNoise(config.map.seed);
        tau = 2 * Math.PI;
        multiplier = 1.0 / tau;
        averageElevation =  config.noise.max_elevation - config.noise.min_elevation;

        noise.SetFrequency((float)config.noise.frequency);
        noise.SetFractalOctaves(config.noise.octaves);
    }

    public override double GetNoise(int x, int y)
    {
            double s = (x / (double)config.map.longitude) * tau;
            double t = (y / (double)config.map.latitude) * tau;
            double nx = Math.Cos(s) * multiplier;
            double ny = Math.Cos(t) * multiplier;
            double nz = Math.Sin(s) * multiplier;
            double nw = Math.Sin(t) * multiplier;

            double n1 = noise.GetPerlinFractal((float) nx,(float) ny);
            double n2 = noise.GetPerlinFractal((float) nz,(float) nw);

            return Math.Min(Math.Max((noise.GetPerlinFractal((float)n1, (float)n2) * averageElevation) + config.noise.min_elevation, config.noise.min_elevation), config.noise.max_elevation);
    }
}
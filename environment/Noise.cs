using System;
public class Noise : NoiseGenerator
{
    private volatile FastNoise noise;

    public Noise(Config config) : base(config){
        noise = new FastNoise(config.seed);
        noise.SetFrequency(config.frequency);
        noise.SetFractalOctaves(100);
    }

    public override double GetNoise(int x, int y)
    {
            float s = x / (float) config.longitude;
            float t = y / (float) config.latitude;
            double nx = Math.Cos(s * 2 * Math.PI) * 1.0 / (2 * Math.PI);
            double ny = Math.Cos(t * 2 * Math.PI) * 1.0 / (2 * Math.PI);
            double nz = Math.Sin(s * 2 * Math.PI) * 1.0 / (2 * Math.PI);
            double nw = Math.Sin(t * 2 * Math.PI) * 1.0 / (2 * Math.PI);

            double n1 = noise.GetSimplexFractal((float) nx,(float) ny);
            double n2 = noise.GetSimplexFractal((float) nz,(float) nw);

            return Math.Min(Math.Max(noise.GetSimplexFractal((float)n1, (float)n2), config.minElevation), config.maxElevation);
    }
}
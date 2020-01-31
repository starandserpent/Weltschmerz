using System;
using System.Runtime.InteropServices;

public class Noise : NoiseGenerator
{
    private static readonly bool USE_EARTH = false;
    private volatile FastNoise noise;
    private int longitude;
    private int latitude;
    private int samples;
    private int terrainMP;
    private int avgTerrain;
    private float frequency;
    private volatile int maxElevation;
    private int minElevation;
    public Noise(Config config){
        Configure(config);
    }

    public override double GetNoise(int x, int y)
    {
            float s = x / (float) longitude;
            float t = y / (float) latitude;
            double nx = Math.Cos(s * 2 * Math.PI) * 1.0 / (2 * Math.PI);
            double ny = Math.Cos(t * 2 * Math.PI) * 1.0 / (2 * Math.PI);
            double nz = Math.Sin(s * 2 * Math.PI) * 1.0 / (2 * Math.PI);
            double nw = Math.Sin(t * 2 * Math.PI) * 1.0 / (2 * Math.PI);

            double n1 = noise.GetSimplexFractal((float) nx,(float) ny);
            double n2 = noise.GetSimplexFractal((float) nz,(float) nw);

            return Math.Min(Math.Max(noise.GetSimplexFractal((float)n1, (float)n2) * terrainMP, minElevation), maxElevation);
    }

    public override void Configure(Config config){
        noise = new FastNoise(config.seed);
        this.terrainMP = config.terrainMP;
        this.avgTerrain = config.avgTerrain;
        this.frequency = config.frequency;
        noise.SetNoiseType(FastNoise.NoiseType.Perlin);
        noise.SetFrequency(frequency);
        noise.SetFractalOctaves(100);
        this.maxElevation = config.maxElevation;
        this.latitude = config.latitude;
        this.longitude = config.longitude;
        this.minElevation = config.minElevation;
    }
}
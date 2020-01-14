using System;

public class Noise
{
    private static readonly bool USE_EARTH = false;
    private volatile FastNoise noise;
    private int longitude;
    private int latitude;
    private int samples;
    private int terrainMP;
    private int avgTerrain;
    private volatile int maxElevation;
    public Noise(Config config){
        ChangeConfig(config);
    }

    public double getNoise(int x, int y)
    {
        if (!USE_EARTH)
        {
            float s = x / (float) longitude;
            float t = y / (float) latitude;
            double nx = Math.Cos(s * 2 * Math.PI) * 1.0 / (2 * Math.PI);
            double ny = Math.Cos(t * 2 * Math.PI) * 1.0 / (2 * Math.PI);
            double nz = Math.Sin(s * 2 * Math.PI) * 1.0 / (2 * Math.PI);
            double nw = Math.Sin(t * 2 * Math.PI) * 1.0 / (2 * Math.PI);
            return Math.Min(Math.Max((noise.GetSimplex((float) nx, (float) ny, (float) nz, (float) nw) * terrainMP) + avgTerrain, 1), maxElevation);
        }
        else
        {
            //return new Color(earth(x, y)).getRed();
            return 0.0;
        }
    }

    public void ChangeConfig(Config config){//, Image earth)
       // this.earth = earth;
        noise = new FastNoise(config.seed);
        this.terrainMP = config.terrainMP;
        this.avgTerrain = config.avgTerrain;
        noise.SetNoiseType(FastNoise.NoiseType.Simplex);
        noise.SetFrequency(config.frequency);
        this.maxElevation = config.maxElevation;
        this.latitude = config.latitude;
        this.longitude = config.longitude;
    }

    public int GetMaxElevation(){
        return maxElevation;
    }
}
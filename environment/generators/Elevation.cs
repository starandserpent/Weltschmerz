using System;
public class Elevation : ElevationGenerator {
    private volatile FastNoise noise;
    private double multiplier;
    private double tau;
    private int averageElevation;
    public Elevation (Weltschmerz weltschmerz, Config config) : base (weltschmerz, config) {
        tau = 2 * Math.PI;
        multiplier = 1.0 / tau;
    }

    public override double GetNoise (int x, int y) {
        double s = (x / (double) config.map.longitude) * tau;
        double t = (y / (double) config.map.latitude) * tau;
        double nx = Math.Cos (s) * multiplier;
        double ny = Math.Cos (t) * multiplier;
        double nz = Math.Sin (s) * multiplier;
        double nw = Math.Sin (t) * multiplier;

        double n1 = noise.GetPerlinFractal ((float) nx, (float) ny);
        double n2 = noise.GetPerlinFractal ((float) nz, (float) nw);

        return Math.Min (Math.Max ((noise.GetPerlinFractal ((float) n1, (float) n2) * averageElevation) + config.elevation.min_elevation, config.elevation.min_elevation), config.elevation.max_elevation);
    }

    public override bool IsLand(double elevation){
        return elevation > config.elevation.water_level;
    }

    public override void Update(){
        noise = new FastNoise (config.map.seed);
        averageElevation = config.elevation.max_elevation - config.elevation.min_elevation;
        noise.SetFrequency ((float) config.elevation.frequency);
        noise.SetFractalOctaves (config.elevation.octaves);
    }

    public override void ChangeConfig(Config config){
        this.config = config;
        Update(); 
    }
}
using System;

/// <summary>
/// Default generator for elevation
/// </summary>
public class Elevation : ElevationGenerator {
    
    //Fast noise from FastNoiseC# library used for generating ladmasses
    private volatile FastNoise noise;

    //Precalculated value used for seamless generation
    private double multiplier;

    //Precalculated value used for seamless generation
    private double tau;

    public Elevation (Weltschmerz weltschmerz, Config config) : base (weltschmerz, config) {
        tau = 2 * Math.PI;
        multiplier = 1.0 / tau;
    }

    public override double GetElevation (int x, int y) {
        //Equation used for generating seamless terrain

        //Default coordinates
        double s = (x / (double) config.map.longitude) * tau;
        double t = (y / (double) config.map.latitude) * tau;

        //First circle
        double nx = Math.Cos (s) * multiplier;
        double ny = Math.Cos (t) * multiplier;

        //Second circle
        double nz = Math.Sin (s) * multiplier;
        double nw = Math.Sin (t) * multiplier;

        double n1 = noise.GetPerlinFractal ((float) nx, (float) ny);
        double n2 = noise.GetPerlinFractal ((float) nz, (float) nw);

        //Placing values from two circles into 2D
        return Math.Min (Math.Max ((Math.Abs(noise.GetPerlinFractal ((float) n1, (float) n2)) * config.elevation.max_elevation ) + config.elevation.min_elevation, config.elevation.min_elevation), config.elevation.max_elevation);
    }

    public override bool IsLand(double elevation){
        return elevation > config.elevation.water_level;
    }

    public override void Update(){
        //Initializes class from FastNoiseC# library
        noise = new FastNoise (config.map.seed);
        noise.SetFrequency ((float) config.elevation.frequency);
        noise.SetFractalOctaves (config.elevation.octaves);
    }

    public override void ChangeConfig(Config config){
        this.config = config;
        Update(); 
    }
}
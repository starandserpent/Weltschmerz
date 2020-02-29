using System.Numerics;
using System;
public class Circulation : CirculationGenerator{
    private Weltschmerz weltschmerz;

    //N*m/(mol*K)
    private double gasConstant = 8.31432;
    //Pa
    private int pressureAtSeaLevel = 101325;

    //kg/mol
    private double molarMass = 0.0289644;

    //m/s^2
    private double gravitionalAcceleration = 9.80665;

    private double temperatureDecrease = 0.0065;

    private double increment;

    public Circulation(Config config, Weltschmerz weltschmerz) : base(config){
        this.weltschmerz = weltschmerz;
        increment = (gravitionalAcceleration * molarMass)/(config.temperature.temperature_decrease * gasConstant);
    }

   public override Vector2 GetAirFlow(int posX, int posY, double pressure, double baseElevation, double elevation){
       Vector2 wind = new Vector2();
       wind.X = (float) pressure;
       wind.Y = (float) CalculateDensity(posX, posY);

        return Vector2.Normalize(wind); 
    }

    public override double CalculateDensity(int posX, int posY) {
        double elevation = weltschmerz.NoiseGenerator.GetNoise(posX, posY);
        double temperature = weltschmerz.TemperatureGenerator.GetTemperatureAtSeaLevel(posY) + 273.15;
        double density = CalculateBaseDensity(posY) * pressureAtSeaLevel;
        if(elevation < 0){
            return density * Math.Pow(1 + (config.temperature.temperature_decrease/temperature) * 11000, increment);
        }
        return density * Math.Pow(1 - (config.temperature.temperature_decrease/temperature) * (elevation - 11000), increment);
    }

    private double CalculateBaseDensity(int posY) {
        double verticallity = (weltschmerz.TemperatureGenerator.GetEquatorDistance(posY)/weltschmerz.TemperatureGenerator.EquatorPosition) * 3;
        return 1.5 - Math.Cos(verticallity * 3);    
    }
}
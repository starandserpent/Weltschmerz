using System.Numerics;
using System;
public class Circulation : CirculationGenerator{
    private Weltschmerz weltschmerz;

    //N*m/(mol*K)
    private static readonly double GAS_CONSTANT = 8.31432;
    //Pa

    //kg/mol
    private static readonly double MOLAR_MASS = 0.0289644;

    //m/s^2
    private static readonly double GRAVITIONAL_ACCELERATION = 9.80665;

    private double increment;

    public Circulation(Config config, Weltschmerz weltschmerz) : base(config){
        this.weltschmerz = weltschmerz;
        increment = (GRAVITIONAL_ACCELERATION * MOLAR_MASS)/(config.temperature.temperature_decrease * GAS_CONSTANT);
    }

   public override Vector2 GetAirFlow(int posX, int posY, double pressure){
       Vector2 wind = new Vector2();
       wind.X = (float) pressure;
       wind.Y = (float) GetAirPressure(posX, posY);

        return Vector2.Normalize(wind); 
    }

    public override double GetAirPressure(int posX, int posY) {
        double elevation = weltschmerz.NoiseGenerator.GetNoise(posX, posY);
        double temperature = weltschmerz.TemperatureGenerator.GetTemperatureAtSeaLevel(posY) + 273.15;
        double density = GetBasePressure(posY) * config.circulation.pressure_at_sea_level;
        if(elevation < 0){
            return density * Math.Pow(1 + (config.temperature.temperature_decrease/temperature) * 11000, increment);
        }
        return density * Math.Pow(1 - (config.temperature.temperature_decrease/temperature) * (elevation - 11000), increment);
    }

    private double GetBasePressure(int posY) {
        double position = (weltschmerz.TemperatureGenerator.GetEquatorDistance(posY)/weltschmerz.TemperatureGenerator.EquatorPosition) * 3;
        return 1.1 - Math.Cos(position * 3);    
    }
}
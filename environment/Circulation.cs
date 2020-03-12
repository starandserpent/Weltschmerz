using System;
using System.Numerics;
public class Circulation : CirculationGenerator {
    private Weltschmerz weltschmerz;

    //N*m/(mol*K)
    private static readonly double GAS_CONSTANT = 8.31432;
    //Pa

    //kg/mol
    private static readonly double MOLAR_MASS = 0.0289644;

    //m/s^2
    private static readonly double GRAVITIONAL_ACCELERATION = 9.80665;

    private double increment;

    public Circulation (Config config, Weltschmerz weltschmerz) : base (config) {
        this.weltschmerz = weltschmerz;
        increment = (GRAVITIONAL_ACCELERATION * MOLAR_MASS) / (weltschmerz.TemperatureGenerator.LapseRate * GAS_CONSTANT);
    }

    public override double GetAirFlow (int posX, int posY, double pressure, double elevation) {
        if (WeltschmerzUtils.IsLand (elevation)) {
            Vector2 wind = new Vector2 ();
            double newPressure = GetAirPressure (posX, posY, elevation);
            wind.X = (float) pressure;
            wind.Y = (float) newPressure;
            wind = Vector2.Normalize (wind);

            return (wind.X - wind.Y);
        } else {
            return 1;
        }
    }

    public override double GetAirPressure (int posX, int posY, double elevation) {
        double temperature = weltschmerz.TemperatureGenerator.GetTemperatureAtSeaLevel (posY) + 273.15;
        double density = GetBasePressure (posY) * config.circulation.pressure_at_sea_level;
        if (elevation <= 0) {
            return density * Math.Pow (1 + (weltschmerz.TemperatureGenerator.LapseRate / temperature) * 11000, increment);
        }

        density *= Math.Pow (1 - (weltschmerz.TemperatureGenerator.LapseRate / temperature) * (elevation - 11000), increment);
        return density;
    }

    private double GetBasePressure (int posY) {
        double position = (weltschmerz.TemperatureGenerator.GetEquatorDistance (posY) / weltschmerz.TemperatureGenerator.EquatorPosition) * 3;
        return 1.5 - Math.Cos (position * 3);
    }
}
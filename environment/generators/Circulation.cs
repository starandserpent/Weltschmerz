using System;
using System.Numerics;

/// <summary>
/// Default generator for circulation
/// </summary>
public class Circulation : CirculationGenerator {

    //Not neccesary enough to place into config (may change in future)
    //Unit is N*m/(mol*K)
    private static readonly double GAS_CONSTANT = 8.31432;

    //Not neccesary enough to place into config (may change in future)
    //Unit is kg/mol
    private static readonly double MOLAR_MASS = 0.0289644;

    //Not neccesary enough to place into config (may change in future)
    //Unit is m/(s^2)
    private static readonly double GRAVITIONAL_ACCELERATION = 9.80665;

    //Precalculated value used for calculating air pressure
    private double increment;

    public Circulation (Weltschmerz weltschmerz, Config config) : base (weltschmerz, config) { }

    public override double GetWindDelta (int posX, int posY, double pressure, bool isLand, double newElevation) {

        if (weltschmerz.ElevationGenerator.IsLand (newElevation) && isLand) {

            //Gets air pressure from another location
            double newPressure = GetAirPressure (posX, posY, newElevation);

            Vector2 wind = new Vector2 ((float) pressure, (float) newPressure);

            //Normalize vector so wind delta can stay between 0 and 1
            wind = Vector2.Normalize (wind);

            //Calculates delta from pressure
            //Higher difference in pressure result in higher wind delta (higher wind speed)
            return (wind.X - wind.Y);
        } else {
            //Wind carries moisture inland 
            if (isLand) {
                return 1;
            }

            return -1;
        }
    }

    public override double GetAirPressure (int posX, int posY, double elevation) {
        //Converts basic temperature to kelvins
        double temperature = weltschmerz.TemperatureGenerator.GetTemperatureAtSeaLevel (posY) + 273.15;

        //Calculates basic air pressure
        double density = GetBasePressure (posY) * config.circulation.pressure_at_sea_level;

        if (elevation <= 0) {
            //Simplified formula for density if elevation is lower then 0
            return density * Math.Pow (1 + (weltschmerz.TemperatureGenerator.LapseRate / temperature) * 11000, increment);
        }

        return density * Math.Pow (1 - (weltschmerz.TemperatureGenerator.LapseRate / temperature) * (elevation - 11000), increment);
    }

    private double GetBasePressure (int posY) {
        //Normalize position up to value 3
        double position = (weltschmerz.TemperatureGenerator.GetEquatorDistance (posY) / weltschmerz.TemperatureGenerator.EquatorPosition) * 3;

        //Estimates pressure based on graph
        return 1.5 - Math.Cos (position * 3);
    }

    public override void Update () {
        //Increment from barometric formula
        increment = (GRAVITIONAL_ACCELERATION * MOLAR_MASS) / (weltschmerz.TemperatureGenerator.LapseRate * GAS_CONSTANT);
    }

    public override void ChangeConfig (Config config) {
        this.config = config;
        Update ();
    }
}
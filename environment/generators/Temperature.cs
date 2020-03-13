using System;

/// <summary>
/// Default generator for temperature
/// </summary>
public class Temperature : TemperatureGenerator {
    public Temperature (Weltschmerz weltschmerz, Config config) : base (weltschmerz, config) {}

    public override double GetEquatorDistance (int posY) {
        return Math.Abs (EquatorPosition - posY);
    }

    public override double GetTemperatureAtSeaLevel (int posY) {
        // Calculates temperature based on equator distance 
        // Lapse rate is used for decreasing temperature with higher latitude
        // he larger distance from equator the lower temperature
        return config.temperature.max_temperature - (GetEquatorDistance (posY) * LapseRate);
    }

    public override double GetTemperature (int posY, double elevation) {

        //Basic temperature on position certain latitude
        double basicTemperature = GetTemperatureAtSeaLevel (posY);

        if (weltschmerz.ElevationGenerator.IsLand (elevation)) {
            //The higher elevation the more temperature decrease
            basicTemperature -= elevation * LapseRate;
        }

        //Makes sure to keep temperature in set levels
        return Math.Min (Math.Max (basicTemperature, config.temperature.min_temperature), config.temperature.max_temperature);
    }

    public override void Update(){
        //Equator position is at half latitude
        this.EquatorPosition = (config.map.latitude / 2.0);

        // Lapse rate is calculated by dividing distance from pole to equator from temperature range
        LapseRate = ((Math.Abs (config.temperature.min_temperature) + config.temperature.max_temperature) / EquatorPosition);
    }

    public override void ChangeConfig(Config config){
        this.config = config;
        Update(); 
    }
}
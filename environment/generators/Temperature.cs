using System;

public class Temperature : TemperatureGenerator {
    //Constructor initializing class with Hocon values
    public Temperature (Weltschmerz weltschmerz, Config config) : base (weltschmerz, config) {}

    /* 
     * Get distance to equator from current position
     * Calculation differens if player is on north or south pole
     */
    public override double GetEquatorDistance (int posY) {
        return Math.Abs (EquatorPosition - posY);
    }

    public override double GetTemperatureAtSeaLevel (int posY) {
        return config.temperature.max_temperature - (GetEquatorDistance (posY) * LapseRate);
    }

    public override double GetTemperature (int posY, double elevation) {

        //The larger distance from equator the lower temperature, if position is on equator it is max temperature
        double basicTemperature = GetTemperatureAtSeaLevel (posY);
        //The higher elevation the more temperature decrease
        if (weltschmerz.ElevationGenerator.IsLand (elevation)) {
            basicTemperature -= elevation * LapseRate;
        }

        //Makes sure to keep temperature in set levels
        return Math.Min (Math.Max (basicTemperature, config.temperature.min_temperature), config.temperature.max_temperature);
    }

    public override void Update(){
        //Equator position is at half world size (latitude)
        this.EquatorPosition = (config.map.latitude / 2.0);

        //Temperature difference sets how much temperature differs per distance from equator (the bigger world the smaller change)
        LapseRate = (float) ((float) (Math.Abs (config.temperature.min_temperature) + config.temperature.max_temperature) / (float) EquatorPosition);
    }

    public override void ChangeConfig(Config config){
        this.config = config;
        Update(); 
    }
}
using System;

public class Temperature : TemperatureGenerator{

    private double tempDifference;

    //Constructor initializing class with Hocon values
    public Temperature(Config config) : base (config){
        //Equator position is at half world size (latitude)
        this.EquatorPosition = (config.map.latitude / 2.0);

        //Temperature difference sets how much temperature differs per distance from equator (the bigger world the smaller change)
        tempDifference = (Math.Abs(config.temperature.min_temperature) + Math.Abs(config.temperature.max_temperature)) / EquatorPosition;
    }

    /* Get distance to equator from current position
    * Calculation differens if player is on north or south pole
    */
    public override double GetEquatorDistance(int posY){

        if(posY < EquatorPosition){
            return Math.Abs(posY - EquatorPosition);
        }

        return Math.Abs(EquatorPosition - posY);
    }

    public override double GetTemperature(int posY, double elevation) {

        //The larger distance from equator the lower temperature, if position is on equator it is max temperature
        double basicTemperature = (GetEquatorDistance(posY) * -tempDifference) + config.temperature.max_temperature;

        //The higher elevation the more temperature decrease
        double decrease = elevation * config.temperature.temperature_decrease;
        if (WeltschmerzUtils.IsLand(elevation)) {
             basicTemperature -= decrease;
        }

        //Makes sure to keep temperature in set levels
        return Math.Min(Math.Max(basicTemperature, config.temperature.min_temperature), config.temperature.max_temperature);
    }
}
using System;

public class Temperature : IConfigurable{
    private int maxTemperature;
    private int minTemperature;
    private double temperatureDecrease;
    private double equatorPosition;
    private double tempDifference;

    //Constructor initializing class with Hocon values
    public Temperature(Config config){
        Configure(config);
    }

    //Copy values from Hocon configuration into class
    public void Configure(Config config){
        this.maxTemperature = config.maxTemperature;
        this.minTemperature = config.minTemperature;
        this.temperatureDecrease = config.temperatureDecrease;

        //Equator position is at half world size (latitude)
        this.equatorPosition = (config.latitude / 2.0);

        //Temperature difference sets how much temperature differs per distance from equator (the bigger world the smaller change)
        tempDifference = (Math.Abs(minTemperature) + Math.Abs(maxTemperature)) / equatorPosition;
    }

    /* Get distance to equator from current position
    * Calculation differens if player is on north or south pole
    */
    public double GetEquatorDistance(int posY){

        if(posY < equatorPosition){
            return Math.Abs(posY - equatorPosition);
        }

        return Math.Abs(equatorPosition - posY);
    }

    public double GetEquatorPosition(){
        return equatorPosition;
    }

    public double GetTemperature(int posY, double elevation) {

        //The larger distance from equator the lower temperature, if position is on equator it is max temperature
        double basicTemperature = (GetEquatorDistance(posY) * -tempDifference) + maxTemperature;

        //The higher elevation the more temperature decrease
        double decrease = elevation * temperatureDecrease;
        if (WeltschmerzUtils.IsLand(elevation)) {
             basicTemperature -= decrease;
        }

        //Makes sure to keep temperature in set levels
        return Math.Min(Math.Max(basicTemperature, minTemperature), maxTemperature);
    }
}
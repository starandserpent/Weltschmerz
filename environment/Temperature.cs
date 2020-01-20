using System;
public class Temperature : IConfigurable{
    private int maxTemperature;
    private int minTemperature;
    private double temperatureDecrease;
    private double equatorPosition;
    public Temperature(Config config){
        Configure(config);
    }
    public void Configure(Config config){
        this.maxTemperature = config.maxTemperature;
        this.minTemperature = config.minTemperature;
        this.temperatureDecrease = config.temperatureDecrease;
        this.equatorPosition = (config.latitude / 2.0);
    }

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
        double tempDifference = (Math.Abs(minTemperature) + Math.Abs(maxTemperature)) / equatorPosition;
        double basicTemperature = (GetEquatorDistance(posY) * -tempDifference) + maxTemperature;

        double decrease = elevation * temperatureDecrease;
        if (elevation > 0) {
            return basicTemperature - decrease;
        } else {
            return basicTemperature;
        }
    }
}
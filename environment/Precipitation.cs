using System.Numerics;
using System;
public class Precipitation : PrecipitationGenerator{
    private Weltschmerz weltschmerz;

    public Precipitation(Config config, Weltschmerz weltschmerz) : base (config){
        this.weltschmerz = weltschmerz;
    }

    public override double GetPrecipitation(int posX, int posY, double elevation, double temperature) {
        double intensity = WeltschmerzUtils.IsLand(elevation) ? config.precipitation.precipitation_intensity : 0;
        double humidity = GetHumidity(posX, posY, elevation);
        double estimated = (1.0 - config.precipitation.circulation_intensity) * GetMoisture(posY) * humidity;
        double temp = (temperature + Math.Abs(config.temperature.min_temperature));
        double simulated = (2.0 * config.precipitation.circulation_intensity) * temp * humidity;
        double precipitation = intensity * (estimated + simulated);
        return Math.Min(Math.Max(precipitation, 0), config.precipitation.max_precipitation);
    }

    public override double GetHumidity(int posX, int posY, double elevation) {
        bool isLand = WeltschmerzUtils.IsLand(elevation);
        double humidity = GetEvapotranspiration(posY, isLand)/(9 * config.circulation.wind_range);

        // calculate humidity
        double pressure = weltschmerz.CirculationGenerator.GetAirPressure(posX, posY);

        for(int x = -config.circulation.wind_range; x <= config.circulation.wind_range; x++){
            for(int y = -config.circulation.wind_range; y <= config.circulation.wind_range; y++){
                int posx = Math.Max(Math.Min(posX + x, config.map.longitude - 1), 0);
                int posy = Math.Max(Math.Min(posY + y, config.map.latitude - 1), 0);

                if(posx != posX && posy != posY){
                double newElevation = weltschmerz.NoiseGenerator.GetNoise(posX, posY);
                double elevationDelta = elevation - newElevation;
                Vector2 vector = new Vector2(x, y);
                // Godot.GD.Print("Before: " + humidity);
                double distance = Math.Sqrt(vector.LengthSquared() + Math.Pow(elevationDelta, 2.0));
               // Godot.GD.Print("Before: " + humidity);
               double evaporation = GetEvapotranspiration(posy, WeltschmerzUtils.IsLand(newElevation))/(9 * config.circulation.wind_range);
               double wind = weltschmerz.CirculationGenerator.GetAirFlow(posx, posy, pressure);
               
               if(wind > 0){
                    humidity += ((evaporation * wind)/distance) + (GetEvapotranspiration(posY, isLand)/(9 * config.circulation.wind_range));
               }else if(wind == 0){
                    humidity += (GetEvapotranspiration(posY, isLand)/(9 * config.circulation.wind_range));
               }else{
                    humidity += (GetEvapotranspiration(posY, isLand)/(9 * config.circulation.wind_range))  + ((evaporation * wind)/distance);
               }

              //  Godot.GD.Print("After: " + humidity);
                }else{
                     humidity += (GetEvapotranspiration(posY, isLand)/(9 * config.circulation.wind_range));
               
                }
            }
        }

        return humidity;
    }

    public override double GetEvapotranspiration(int posY, bool isLand) {
        double evapotranspiration;
        if (isLand) {
            evapotranspiration = config.humidity.transpiration;
        } else {
            evapotranspiration = config.humidity.evaporation;
        }

        evapotranspiration *= GetMoisture(posY);
        return evapotranspiration;
    }

    public override double GetMoisture(int posY) {
        double y = posY/weltschmerz.TemperatureGenerator.EquatorPosition;
        double moisture =  2 - (Math.Cos(y * Math.PI) + Math.Cos(3*y*Math.PI))/2;
        return moisture * config.precipitation.max_precipitation;
    }
}
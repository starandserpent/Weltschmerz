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
        double estimated = (1.0 - config.precipitation.circulation_intensity) * GetMoisture(posY);
        double temp = temperature/(config.temperature.max_temperature + Math.Abs(config.temperature.min_temperature));
        double simulated = (2.0 * config.precipitation.circulation_intensity) * humidity;
        double precipitation = intensity * (estimated + simulated);
        return Math.Min(Math.Max(precipitation, 0), config.precipitation.max_precipitation);
    }

    public override double GetHumidity(int posX, int posY, double elevation) {
        bool isLand = WeltschmerzUtils.IsLand(elevation);
        double humidity = GetEvapotranspiration(posY, isLand)/(2 * config.circulation.wind_range);

        // calculate humidity
        double pressure = weltschmerz.CirculationGenerator.GetAirPressure(posX, posY);

        for(int i = 1; i <= config.circulation.wind_range; i++){
                int posx = Math.Max(posX - i, 0);
                int posy = Math.Min(posY + i, config.map.latitude - 1);

                if(posx != posX && posy != posY){
                double newElevation = weltschmerz.NoiseGenerator.GetNoise(posx, posy);
                double elevationDelta = elevation - newElevation;
                Vector2 vector = new Vector2(i, i);

                double distance = Math.Sqrt(vector.LengthSquared() + Math.Pow(elevationDelta, 2.0));
               double evaporation = GetEvapotranspiration(posy, WeltschmerzUtils.IsLand(newElevation))/(2 * config.circulation.wind_range);
               double wind = weltschmerz.CirculationGenerator.GetAirFlow(posx, posy, pressure, newElevation);

               if(wind > 0){
                    humidity += ((evaporation * wind)/distance) + (GetEvapotranspiration(posY, isLand)/(2 * config.circulation.wind_range));
               }else if(wind == 0){
                    humidity += (GetEvapotranspiration(posY, isLand)/(2 * config.circulation.wind_range));
               }else{
                    humidity += (GetEvapotranspiration(posY, isLand)/(2 * config.circulation.wind_range))  + ((evaporation * wind)/distance);
               }

                 }else{
                     humidity += (GetEvapotranspiration(posY, isLand)/(2 * config.circulation.wind_range));
                }
            
                posx = Math.Min(posX + i, config.map.longitude - 1);
                posy = Math.Max(posY - i, 0);
                if(posx != posX && posy != posY){
                double newElevation = weltschmerz.NoiseGenerator.GetNoise(posx, posy);
                double elevationDelta = elevation - newElevation;
                Vector2 vector = new Vector2(i, i);
                double distance = Math.Sqrt(vector.LengthSquared() + Math.Pow(elevationDelta, 2.0));
               // Godot.GD.Print("Before: " + humidity);
               double evaporation = GetEvapotranspiration(posy, WeltschmerzUtils.IsLand(newElevation))/(2 * config.circulation.wind_range);
               double wind = weltschmerz.CirculationGenerator.GetAirFlow(posx, posy, pressure, newElevation);

               if(wind > 0){
                    humidity += ((evaporation * wind)/distance) + (GetEvapotranspiration(posY, isLand)/(2 * config.circulation.wind_range));
               }else if(wind == 0){
                    humidity += (GetEvapotranspiration(posY, isLand)/(2 * config.circulation.wind_range));
               }else{
                    humidity += (GetEvapotranspiration(posY, isLand)/(2 * config.circulation.wind_range))  + ((evaporation * wind)/distance);
               }
                }else{
                     humidity += (GetEvapotranspiration(posY, isLand)/(2 * config.circulation.wind_range));
                }
              //  Godot.GD.Print("After: " + humidity);
        }

        return Math.Max(humidity, 0);
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
        double moisture = ((1.0/3.0)-Math.Cos(y * Math.PI)) + ((1.0/3.0) - Math.Cos(3*y*Math.PI));
        return moisture * config.precipitation.max_precipitation/3;
    }
}
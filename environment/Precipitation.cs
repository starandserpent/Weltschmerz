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
        double humidity = GetEvapotranspiration(posY, isLand);

        double intensity = isLand ? config.precipitation.precipitation_intensity : 0;

        // calculate humidity
        double pressure = weltschmerz.CirculationGenerator.GetAirPressure(posX, posY);

        for(int x = -3; x <= 3; x++){
            for(int y = -3; y <= 3; y++){
                int posx = Math.Max(Math.Min(posX + x, config.map.longitude - 1), 0);
                int posy = Math.Max(Math.Min(posY + y, config.map.latitude - 1), 0);
                Vector2 vector = new Vector2(posx, posy);
                humidity += WindExchange(posx, posy, pressure, elevation)/(vector.Length() + 1);
            }
        }

        return humidity;
    }

    private double WindExchange(int posX, int posY, double basePressure, double baseElevation){
        double elevation = weltschmerz.NoiseGenerator.GetNoise(posX, posY);
        Vector2 wind = weltschmerz.CirculationGenerator.GetAirFlow(posX, posY, basePressure);
        bool isLand = WeltschmerzUtils.IsLand(elevation);
        double airExchange = GetEvapotranspiration(posY, isLand);
        airExchange *= (wind.X - wind.Y);
        return airExchange * config.circulation.wind_intensity;
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
        double moisture =  1 - (Math.Cos(y * Math.PI) + Math.Cos(3*y*Math.PI))/2;
        return moisture * config.precipitation.max_precipitation;
    }
}
using System.Numerics;
using System;
public class Precipitation : PrecipitationGenerator{
    private Weltschmerz weltschmerz;

    public Precipitation(Config config, Weltschmerz weltschmerz) : base (config){
        this.weltschmerz = weltschmerz;
    }

    private Vector3 GetElevationGradient(int posX, int posY) {
        float x = Math.Max(Math.Min(posX + config.precipitation.elevation_delta, config.map.longitude - 1), 0);
        float y = Math.Max(Math.Min(posY + config.precipitation.elevation_delta, config.map.latitude - 1), 0);

        x = (float) (weltschmerz.NoiseGenerator.GetNoise(Math.Min((int) x + config.precipitation.elevation_delta, config.map.longitude - 1), posY)
                - weltschmerz.NoiseGenerator.GetNoise(Math.Max((int) x - config.precipitation.elevation_delta, 0), posY));
        y = (float) (weltschmerz.NoiseGenerator.GetNoise(posX, Math.Min((int) y + config.precipitation.elevation_delta, config.map.latitude - 1))
                - weltschmerz.NoiseGenerator.GetNoise(posX, Math.Max((int) y - config.precipitation.elevation_delta, 0)));

        return Vector3.Normalize(new Vector3(x, 0.01F * config.precipitation.elevation_delta, y));
    }

    public override double GetPrecipitation(int posX, int posY, double elevation, double temperature, Vector2 wind) {
        double intensity = WeltschmerzUtils.IsLand(elevation) ? config.precipitation.precipitationI_intensity : 0;
        double humidity = GetHumidity(posX, posY, wind, elevation);
        double estimated = (1.0 - config.precipitation.circulation_intensity) * GetMoisture(posY) * humidity;
        double elevationGradient = GetElevationGradient(posX, posY).Y;
        double temp = (temperature + Math.Abs(config.temperature.min_temperature)) + GetOrotographicEffect(elevation, elevationGradient, wind, config.precipitation.orographic_effect);
        double simulated = (2.0 * config.precipitation.circulation_intensity) * temp * humidity;
        double precipitation = intensity * (estimated + simulated);
        return Math.Min(Math.Max(precipitation, 0), config.precipitation.max_precipitation);
    }

    private double GetHumidity(int posX, int posY, Vector2 wind, double elevation) {
        bool isLand = WeltschmerzUtils.IsLand(elevation);
        double humidity = GetEvapotranspiration(posY, isLand);
        double elevationGradient = GetElevationGradient(posX, posY).Y;

        double finalOrographicEffect = GetOrotographicEffect(elevation, elevationGradient, wind, config.precipitation.orographic_effect);
        double inverseOrographicEffect = 1.0 - finalOrographicEffect;

        double intensity = isLand ? 1.0 * config.precipitation.precipitationI_intensity : 0;
        double scale = config.precipitation.iteration;

        // circulate humidity
        double x = Math.Max(Math.Min(posX - (Vector2.Normalize(wind).X * wind.Length() * scale), config.map.longitude - 1), 0.0);
        double y = Math.Max(Math.Min(posY - (Vector2.Normalize(wind).Y * wind.Length() * scale), config.map.latitude - 1), 0.0);

        double inflowHumidity = GetEvapotranspiration((int)y, WeltschmerzUtils.IsLand(weltschmerz.NoiseGenerator.GetNoise((int)x,(int) y)));

        x = Math.Max(Math.Min(posX + (Vector2.Normalize(wind).X * wind.Length() * scale), (double)config.map.longitude - 1), 0.0);
        y = Math.Max(Math.Min(posY + (Vector2.Normalize(wind).Y * wind.Length() * scale), (double)config.map.latitude - 1), 0.0);

        double outflowHumidity = GetEvapotranspiration((int)y, WeltschmerzUtils.IsLand(weltschmerz.NoiseGenerator.GetNoise((int)x,(int) y)));

        double inflow = inflowHumidity - humidity;
        double outflow = humidity - outflowHumidity;
        humidity += inflow * intensity * inverseOrographicEffect;
        humidity -= outflow * intensity;

        return Math.Max(humidity, 0.1);
    }

    private double GetEvapotranspiration(int posY, bool isLand) {
        double evapotranspiration;
        if (isLand) {
            evapotranspiration = config.humidity.transpiration;
        } else {
            evapotranspiration = config.humidity.evaporation;
        }

        evapotranspiration *= GetMoisture(posY);
        return evapotranspiration;
    }

    private static double GetOrotographicEffect(double elevation, double elevationGradient, Vector2 wind, double ortographicEffect) {
        wind = Vector2.Normalize(wind);
        double slope = WeltschmerzUtils.IsLand(elevation) ? 1.0 - elevationGradient : 0.0;
        double uphill = Math.Max(Math.Min(wind.X * -elevation, wind.Y * -elevation), 0.0);
        return uphill * slope * ortographicEffect;
    }

    public double GetMoisture(int posY) {
        double y = posY/weltschmerz.TemperatureGenerator.EquatorPosition;
        double moisture =  1 - (Math.Cos(y * Math.PI) + Math.Cos(3*y*Math.PI))/2;
        return moisture * config.precipitation.max_precipitation/2;
    }
}
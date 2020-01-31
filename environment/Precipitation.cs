using System.Numerics;
using System;
public class Precipitation : PrecipitationGenerator{
    private int longitude;
    private int latitude;
    private double zoom;
    private double change;
    private double moistureIntensity;
    private double circulationIntensity;
    private double orographicEffect;
    private double precipitationIntensity;
    private double iteration;
    private double transpiration;
    private double evaporation;
    private int minTemperature;
    private Weltschmerz weltschmerz;
    private int elevationDelta;

    private int maxPrecipitation;
    public Precipitation(Config config, Weltschmerz weltschmerz){
        Configure(config);
        this.weltschmerz = weltschmerz;
    }

    private Vector3 GetElevationGradient(int posX, int posY) {
        float x = Math.Max(Math.Min(posX + elevationDelta, longitude - 1), 0);
        float y = Math.Max(Math.Min(posY + elevationDelta, latitude - 1), 0);

        x = (float) (weltschmerz.NoiseGenerator.GetNoise(Math.Min((int) x + elevationDelta, longitude - 1), posY)
                - weltschmerz.NoiseGenerator.GetNoise(Math.Max((int) x - elevationDelta, 0), posY));
        y = (float) (weltschmerz.NoiseGenerator.GetNoise(posX, Math.Min((int) y + elevationDelta, latitude - 1))
                - weltschmerz.NoiseGenerator.GetNoise(posX, Math.Max((int) y - elevationDelta, 0)));

        return Vector3.Normalize(new Vector3(x, 0.01F * elevationDelta, y));
    }

    public override double GetPrecipitation(int posX, int posY, double elevation, double temperature, Vector2 wind) {
        double intensity = WeltschmerzUtils.IsLand(elevation) ? precipitationIntensity : 0;
        double humidity = GetHumidity(posX, posY, wind, elevation);
        double estimated = (1.0 - circulationIntensity) * GetEstimatedMoisture(posY) * humidity;
        double elevationGradient = GetElevationGradient(posX, posY).Y;
        double temp = (temperature + Math.Abs(minTemperature)) + GetOrotographicEffect(elevation, elevationGradient, wind, orographicEffect);
        double simulated = (2.0 * circulationIntensity) * temp * humidity;
        double precipitation = intensity * (estimated + simulated);
        return Math.Max(precipitation, 0);
    }

    private double GetHumidity(int posX, int posY, Vector2 wind, double elevation) {
        bool isLand = WeltschmerzUtils.IsLand(elevation);
        double humidity = GetEvapotranspiration(posY, isLand);
        double elevationGradient = GetElevationGradient(posX, posY).Y;

        double finalOrographicEffect = GetOrotographicEffect(elevation, elevationGradient, wind, orographicEffect);
        double inverseOrographicEffect = 1.0 - finalOrographicEffect;

        double intensity = isLand ? 1.0 * precipitationIntensity : 0;
        double scale = iteration;

        // circulate humidity
        double x = Math.Max(Math.Min(posX - (Vector2.Normalize(wind).X * wind.Length() * scale), longitude - 1), 0.0);
        double y = Math.Max(Math.Min(posY - (Vector2.Normalize(wind).Y * wind.Length() * scale), latitude - 1), 0.0);

        double inflowHumidity = GetEvapotranspiration((int)y, WeltschmerzUtils.IsLand(weltschmerz.NoiseGenerator.GetNoise((int)x,(int) y)));

        x = Math.Max(Math.Min(posX + (Vector2.Normalize(wind).X * wind.Length() * scale), (double)longitude - 1), 0.0);
        y = Math.Max(Math.Min(posY + (Vector2.Normalize(wind).Y * wind.Length() * scale), (double)latitude - 1), 0.0);

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
            evapotranspiration = transpiration;
        } else {
            evapotranspiration = evaporation;
        }

        evapotranspiration *= GetEstimatedMoisture(posY);
        return evapotranspiration;
    }

    private static double GetOrotographicEffect(double elevation, double elevationGradient, Vector2 wind, double ortographicEffect) {
        wind = Vector2.Normalize(wind);
        double slope = WeltschmerzUtils.IsLand(elevation) ? 1.0 - elevationGradient : 0.0;
        double uphill = Math.Max(Math.Min(wind.X * -elevation, wind.Y * -elevation), 0.0);
        return uphill * slope * ortographicEffect;
    }

    private double GetEstimatedMoisture(int posY) {
        double y = posY/weltschmerz.TemperatureGenerator.EquatorPosition;
        double moisture =  1 - (Math.Cos(y * Math.PI) + Math.Cos(3*y*Math.PI))/2;
        return moisture * maxPrecipitation/2;
    }

    public override void Configure(Config config){
        this.latitude = config.latitude;
        this.longitude = config.longitude;
        this.zoom = config.zoom;   
        this.change = config.change;
        this.moistureIntensity = config.moistureIntensity;
        this.circulationIntensity = config.circulationIntensity;
        this.orographicEffect = config.orographicEffect;
        this.precipitationIntensity = config.precipitationIntensity;
        this.iteration = config.iteration;
        this.transpiration = config.transpiration;
        this.evaporation = config.evaporation;
        this.elevationDelta = config.elevationDelta;
        this.minTemperature = config.minTemperature;
        this.maxPrecipitation = config.maxPrecipitation;
    }
}
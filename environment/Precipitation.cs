using System.Numerics;
using System;
public class Precipitation : IConfigurable{
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
    private Temperature equator;
    private Noise noise;
    private double intensity;
    private int elevationDelta;
    public Precipitation(Config config, Noise noise, Temperature equator){
        Configure(config);
        this.noise = noise;
        this.equator = equator;
    }

    private Vector3 getElevationGradient(int posX, int posY) {
        float x = Math.Min(Math.Min(posX + elevationDelta, longitude - 1), 0);
        float y = Math.Min(Math.Min(posY + elevationDelta, latitude - 1), 0);

        x = (float) (noise.GetNoise(Math.Min((int) x + elevationDelta, longitude - 1), posY)
                - noise.GetNoise(Math.Max((int) x - elevationDelta, 0), posY));
        y = (float) (noise.GetNoise(posX, Math.Min((int) y + elevationDelta, latitude - 1))
                - noise.GetNoise(posX, Math.Max((int) y - elevationDelta, 0)));

        return Vector3.Normalize(new Vector3(x, 0.01F * elevationDelta, y));
    }

    public double GetMoisture(int posY) {
        double verticality = (WeltschmerzUtils.ToUnsignedRange(equator.GetEquatorDistance(posY)) / zoom);
        double mix = WeltschmerzUtils.Mix(-Math.Cos(verticality * 3 * Math.PI * 2),
                -Math.Cos(verticality * Math.PI * 2), change);
        return WeltschmerzUtils.ToUnsignedRange(Math.Abs(mix) * moistureIntensity);
    }

    public double GetPrecipitation(int posX, int posY, double elevation, double temperature, Vector2 wind) {
        double humidity = GetHumidity(posX, posY, wind, elevation);
        double estimated = (1.0 - circulationIntensity) * GetBasePrecipitation(posY);
        double elevationGradient = getElevationGradient(posX, posY).Y;
        double simulated = (2.0 * circulationIntensity) * (temperature + 10 + GetOrotographicEffect(elevation, elevationGradient, wind,
                orographicEffect)) * humidity;
        return Math.Max(intensity * (estimated + simulated), 0);
    }

    private double GetHumidity(int posX, int posY, Vector2 wind, double elevation) {
        bool isLand = WeltschmerzUtils.IsLand(elevation);
        double humidity = GetEvapotranspiration(posY, isLand);
        double elevationGradient = getElevationGradient(posX, posY).Y;

        double finalOrographicEffect = GetOrotographicEffect(elevation, elevationGradient, wind, orographicEffect);
        double inverseOrographicEffect = 1.0 - finalOrographicEffect;

        intensity = isLand ? 1.0 * precipitationIntensity : 0;
        double scale = iteration * 0.01;

        // circulate humidity
        int x = Math.Max(Math.Min((int) (posX - (Vector2.Normalize(wind).X * wind.Length() * scale)), longitude - 1), 0);
        int y = Math.Max(Math.Min((int) (posY - (Vector2.Normalize(wind).Y * wind.Length() * scale)), latitude - 1), 0);

        double inflowHumidity = GetEvapotranspiration(y, WeltschmerzUtils.IsLand(noise.GetNoise(x, y)));

        x = Math.Max(Math.Min((int) (posX + (Vector2.Normalize(wind).X * wind.Length() * scale)), longitude - 1), 0);
        y = Math.Max(Math.Min((int) (posY + (Vector2.Normalize(wind).Y * wind.Length() * scale)), latitude - 1), 0);

        double outflowHumidity = GetEvapotranspiration(y, WeltschmerzUtils.IsLand(noise.GetNoise(x, y)));

        double inflow = Math.Max(inflowHumidity - humidity, 0.0);
        double outflow = Math.Max(humidity - outflowHumidity, 0.0);
        humidity += inflow * intensity * inverseOrographicEffect;
        humidity -= outflow * intensity;

        return humidity;
    }

    private double GetEvapotranspiration(int posY, bool isLand) {
        double evapotranspiration;
        if (isLand) {
            evapotranspiration = transpiration;
        } else {
            evapotranspiration = evaporation;
        }

        evapotranspiration *= GetMoisture(posY);
        return evapotranspiration;
    }

    private static double GetOrotographicEffect(double elevation, double elevationGradient, Vector2 wind, double ortographicEffect) {
        wind = Vector2.Normalize(wind);
        double slope = WeltschmerzUtils.IsLand(elevation) ? 1.0 - elevationGradient : 0.0;
        double uphill = Math.Max(Math.Max(wind.X * -elevation, wind.Y * -elevation), 0.0);
        return uphill * slope * ortographicEffect;
    }

    private double GetBasePrecipitation(int posY) {
        double verticality = WeltschmerzUtils.ToUnsignedRange(equator.GetEquatorDistance(posY));
        return WeltschmerzUtils.ToUnsignedRange(-Math.Cos(verticality * 3 * Math.PI * 2));
    }

    public void Configure(Config config){
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
    }
}
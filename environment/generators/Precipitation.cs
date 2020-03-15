using System;
using System.Numerics;

/// <summary>
/// Default generator for precipitation
/// </summary>
public class Precipitation : PrecipitationGenerator {
    public Precipitation (Weltschmerz weltschmerz, Config config) : base (weltschmerz, config) { }

    public override double GetPrecipitation (int posX, int posY, double elevation, double temperature) {
        double humidity = GetHumidity (posX, posY, elevation);
        double estimated = (1.0 - config.precipitation.circulation_intensity) * GetMoisture (posY);
        double simulated = (2.0 * config.precipitation.circulation_intensity) * humidity;
        double precipitation = config.precipitation.precipitation_intensity * (estimated + simulated);
        return Math.Min (Math.Max (precipitation, 0), config.precipitation.max_precipitation);
    }

    public override double GetHumidity (int posX, int posY, double elevation) {
        bool isLand = weltschmerz.ElevationGenerator.IsLand (elevation);

        //Get part of base humidity
        double humidity = GetEvapotranspiration (posY, isLand) / (2 * config.circulation.wind_range);

        // calculate humidity
        double pressure = weltschmerz.CirculationGenerator.GetAirPressure (posX, posY, elevation);

        for (int i = 1; i <= config.circulation.wind_range; i++) {
            Vector2 vector = new Vector2 (i, i);

            if (weltschmerz.TemperatureGenerator.EquatorPosition < posY) {

                //Caculate transfer position from north-east            
                int posx = Math.Min (posX + i, config.map.longitude - 1);
                int posy = Math.Min (posY + i, config.map.latitude - 1);

                humidity += GetHumidityTrasfer (posx, posy, elevation, pressure, isLand, vector);

                //Caculate transfer position from south-west
                posx = Math.Max (posX - i, 0);
                posy = Math.Max (posY - i, 0);

                humidity += GetHumidityTrasfer (posx, posy, elevation, pressure, isLand, vector);
            } else {
                //Caculate transfer position from north-east
                int posx = Math.Max (posX - i, 0);
                int posy = Math.Min (posY + i, config.map.latitude - 1);

                humidity += GetHumidityTrasfer (posx, posy, elevation, pressure, isLand, vector);

                //Caculate transfer position from south-west
                posx = Math.Min (posX + i, config.map.longitude - 1);
                posy = Math.Max (posY - i, 0);

                humidity += GetHumidityTrasfer (posx, posy, elevation, pressure, isLand, vector);
            }
        }

        return Math.Max (humidity, 0);
    }

    //Method used to calculate humidity transfer between location
    private double GetHumidityTrasfer (int posX, int posY, double elevation, double pressure, bool isLand, Vector2 vector) {
        double newElevation = weltschmerz.ElevationGenerator.GetElevation (posX, posY);
        double elevationDelta = elevation - newElevation;
        double distance = Math.Sqrt (vector.LengthSquared () + Math.Pow (elevationDelta, 2.0));

        double evaporation = GetEvapotranspiration (posY, weltschmerz.ElevationGenerator.IsLand (newElevation)) / (2 * config.circulation.wind_range);
        double wind = weltschmerz.CirculationGenerator.GetWindDelta (posX, posY, pressure, isLand, newElevation);

        if (wind == 0) {
            return (GetEvapotranspiration (posY, isLand) / (2 * config.circulation.wind_range));
        } else {
            return (GetEvapotranspiration (posY, isLand) / (2 * config.circulation.wind_range)) + ((evaporation * wind) / distance);
        }
    }

    public override double GetEvapotranspiration (int posY, bool isLand) {
        //Gets basic moisture
        double evapotranspiration = GetMoisture (posY);

        //Evapotraspiration is multiplied transpiration (landmass) or evaporation (ocean)
        if (isLand) {
            return evapotranspiration * config.humidity.transpiration;
        }

        return evapotranspiration * config.humidity.evaporation;
    }

    public override double GetMoisture (int posY) {
        //Normalize postion to max 2
        double y = posY / weltschmerz.TemperatureGenerator.EquatorPosition;

        //Estimates moisture based on graph
        double moisture = ((1.0 / 3.0) - Math.Cos (y * Math.PI)) + ((1.0 / 3.0) - Math.Cos (3 * y * Math.PI));

        //Sets precipitation to max value
        return moisture * config.precipitation.max_precipitation / 3;
    }

    public override void Update () { }

    public override void ChangeConfig (Config config) {
        this.config = config;
        Update ();
    }
}
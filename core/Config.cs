public class Config{

    //Map
    public int seed {get; set;}
    public int latitude {get; set;}
    public int longitude {get; set;}
    public int maxElevation {get; set;}
    public int minElevation {get; set;}

    //Noise
    public int terrainMP {get; set;}
    public int avgTerrain {get; set;}
    public float frequency {get; set;}

    //Temperature
    public int maxTemperature {get; set;}
    public int minTemperature {get; set;}
    public float temperatureDecrease {get; set;}

    //Precipitation
    public float orographicEffect {get; set;}
    public float circulationIntensity {get; set;}
    public float precipitationIntensity {get; set;}
    public float iteration {get; set;}
    public int elevationDelta {get; set;}
    public int maxPrecipitation {get; set;}
    public int minPrecipitation {get; set;}

    //Humidity
    public float transpiration {get; set;}
    public float evaporation {get; set;}
    
    //Circulation
    public float exchangeCoefficient {get; set;}
    public int circulationOctaves {get; set;}
    public float temperatureInfluence {get; set;}
    public int circulationDecline {get; set;}
}
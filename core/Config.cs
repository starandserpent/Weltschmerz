public struct Config{

    //Map
    public int seed {get; set;}
    public int latitude {get; set;}
    public int longitude {get; set;}
    public int maxElevation {get; set;}

    //Noise
    public int terrainMP {get; set;}
    public int avgTerrain {get; set;}
    public float frequency {get; set;}

    //Temperature
    public int maxTemperature {get; set;}
    public int minTemperature {get; set;}
    public float temperatureDecrease {get; set;}

    //Moisture
    public float zoom {get; set;}
    public float moistureIntensity {get; set;}
    public float change {get; set;}

    //Precipitation
    public float orographicEffect {get; set;}
    public float circulationIntensity {get; set;}
    public float precipitationIntensity {get; set;}
    public float iteration {get; set;}
    public int elevationDelta {get; set;}

    //Humidity
    public float transpiration {get; set;}
    public int evaporation {get; set;}
    
    //Circulation
    public float exchangeCoefficient {get; set;}
    public int circulationOctaves {get; set;}
    public float temperatureInfluence {get; set;}
    public int circulationDecline {get; set;}
}
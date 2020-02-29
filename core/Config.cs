public class Config{

    public Map map {get;}
    public Noise noise {get;}
    public Temperature temperature {get;}
    public Precipitation precipitation {get;}
    public Humidity humidity {get;}
    public Circulation circulation {get;}


    public Config(){
        map = new Map();
        noise = new Noise();
        temperature = new Temperature();
        precipitation = new Precipitation();
        humidity = new Humidity();
        circulation = new Circulation();
    }

    //Map
    public class Map{
        public int seed {get; set;}
        public int latitude {get; set;}
        public int longitude {get; set;}
    }

    //Noise
    public class Noise{
        public int max_elevation {get; set;}
        public int min_elevation {get; set;}
        public int terrainMP {get; set;}
        public int avgTerrain {get; set;}
        public float frequency {get; set;}
    }

    //Temperature
    public class Temperature{
        public int max_temperature {get; set;}
        public int min_temperature {get; set;}
        public float temperature_decrease {get; set;}
    }

    //Precipitation
    public class Precipitation{
        public float orographic_effect {get; set;}
        public float circulation_intensity {get; set;}
        public float precipitation_intensity {get; set;}
        public float iteration {get; set;}
        public int elevation_delta {get; set;}
        public int max_precipitation {get; set;}
        public int min_precipitation {get; set;}
    }

    //Humidity
    public class Humidity{
        public float transpiration {get; set;}
        public float evaporation {get; set;}
    }
    
    //Circulation
    public class Circulation{
        public float exchange_coefficient {get; set;}
        public int circulation_octaves {get; set;}
        public int circulation_decline {get; set;}
    }
}
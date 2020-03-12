public class Config {

    public Map map { get; }
    public Noise noise { get; }
    public Temperature temperature { get; }
    public Precipitation precipitation { get; }
    public Humidity humidity { get; }
    public Circulation circulation { get; }

    public Config () {
        map = new Map ();
        noise = new Noise ();
        temperature = new Temperature ();
        precipitation = new Precipitation ();
        humidity = new Humidity ();
        circulation = new Circulation ();
    }

    //Map
    public class Map {
        public int seed { get; set; }
        public int latitude { get; set; }
        public int longitude { get; set; }
    }

    //Noise
    public class Noise {
        public int max_elevation { get; set; }
        public int min_elevation { get; set; }
        public float frequency { get; set; }
        public int octaves { get; set; }
    }

    //Temperature
    public class Temperature {
        public int max_temperature { get; set; }
        public int min_temperature { get; set; }
    }

    //Precipitation
    public class Precipitation {
        public float circulation_intensity { get; set; }
        public float precipitation_intensity { get; set; }
        public int max_precipitation { get; set; }
    }

    //Humidity
    public class Humidity {
        public float transpiration { get; set; }
        public float evaporation { get; set; }
    }

    //Circulation
    public class Circulation {
        public int wind_range { get; set; }
        public float wind_intensity { get; set; }
        public int pressure_at_sea_level { get; set; }
    }
}
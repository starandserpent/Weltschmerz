using System;
using System.IO;
using Akka.Configuration;
using AkkaConfig = Akka.Configuration.Config;
public class ConfigManager{
    public static readonly string CONFIG_NAME = "config.conf";
    public static readonly string BASE_DIRECTORY = AppDomain.CurrentDomain.BaseDirectory;
    public static readonly string BASE_CONFIG_PATH = GetConfigFilePath("*"+CONFIG_NAME, BASE_DIRECTORY);
    public static Config GetConfig(){
       return GetConfig(BASE_CONFIG_PATH);
;
    }
    public static Config GetConfig(string path){

        Config config = new Config();

        AkkaConfig hocon = ConfigurationFactory.ParseString(File.ReadAllText(path));
        
        //Map
        config.seed = hocon.GetInt("map.seed");
        config.latitude = hocon.GetInt("map.latitude");
        config.longitude = hocon.GetInt("map.longitude");

        //Noise
        config.avgTerrain = hocon.GetInt("noise.average_terrain_hight");
        config.frequency = hocon.GetFloat("noise.frequency");
        config.terrainMP = hocon.GetInt("noise.terrain_multiplaier");
        config.maxElevation = hocon.GetInt("noise.max_elevation");
        config.minElevation = hocon.GetInt("noise.min_elevation");

        //Temperature
        config.maxTemperature  = hocon.GetInt("temperature.max_temperature");
        config.minTemperature  = hocon.GetInt("temperature.min_temperature");
        config.temperatureDecrease  = hocon.GetFloat("temperature.temperature_decrease");

        //Precipitation
        config.orographicEffect  = hocon.GetFloat("precipitation.orographic_effect");
        config.circulationIntensity  = hocon.GetFloat("precipitation.circulation_intensity");
        config.precipitationIntensity  = hocon.GetFloat("precipitation.precipitation_intensity");
        config.iteration  = hocon.GetFloat("precipitation.iteration");
        config.elevationDelta  = hocon.GetInt("precipitation.elevation_delta");
        config.maxPrecipitation  = hocon.GetInt("precipitation.max_precipitation");

        //Humidity
        config.transpiration  = hocon.GetFloat("humidity.transpiration");
        config.evaporation  = hocon.GetFloat("humidity.evaporation");
    
        //Circulation
        config.exchangeCoefficient  = hocon.GetFloat("circulation.exchange_coefficient");
        config.circulationOctaves  = hocon.GetInt("circulation.circulation_octaves");
        config.circulationDecline  = hocon.GetInt("circulation.circulation_decline");

        return config;
    }

    public static string GetConfigFilePath(string fileName, string path){
        DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo(path);
        FileInfo[] filesInDir = hdDirectoryInWhichToSearch.GetFiles(fileName, SearchOption.AllDirectories);

        string fullName = "";
        foreach (FileInfo foundFile in filesInDir)
        {
            fullName = foundFile.FullName;
        }
        
        return fullName;
    }
}
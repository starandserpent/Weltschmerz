using System;
using System.IO;
using Akka.Configuration;
using AkkaConfig = Akka.Configuration.Config;
public class ConfigManager{
    public static Config GetConfig(){
        DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
        FileInfo[] filesInDir = hdDirectoryInWhichToSearch.GetFiles("*config.conf", SearchOption.AllDirectories);

        string fullName = "";
        foreach (FileInfo foundFile in filesInDir)
        {
            fullName = foundFile.FullName;
        }
        
        Config config = new Config();

        AkkaConfig hocon = ConfigurationFactory.ParseString(File.ReadAllText(fullName));
        
        //Map
        config.seed = hocon.GetInt("map.seed");
        config.latitude = hocon.GetInt("map.latitude");
        config.longitude = hocon.GetInt("map.longitude");
        config.maxElevation = hocon.GetInt("map.max_elevation");

        //Noise
        config.avgTerrain = hocon.GetInt("noise.average_terrain_hight");
        config.frequency = hocon.GetFloat("noise.frequency");
        config.terrainMP = hocon.GetInt("noise.terrain_multiplaier");

        //Temperature
    config.maxTemperature  = hocon.GetInt("temperature.max_temperature");
     config.minTemperature  = hocon.GetInt("temperature.min_temperature");
    config.temperatureDecrease  = hocon.GetFloat("temperature.temperature_decrease");

    //Moisture
    config.zoom  = hocon.GetFloat("moisture.zoom");
    config.moistureIntensity  = hocon.GetFloat("moisture.moisture_intensity");
   config.change  = hocon.GetFloat("moisture.change");

    //Precipitation
    config.orographicEffect  = hocon.GetFloat("precipitation.circulation_intensity");
    config.circulationIntensity  = hocon.GetFloat("precipitation.orographic_effect");
    config.precipitationIntensity  = hocon.GetFloat("precipitation.precipitation_intensity");
    config.iteration  = hocon.GetFloat("precipitation.iteration");
    config.elevationDelta  = hocon.GetInt("precipitation.elevation_delta");

    //Humidity
    config.transpiration  = hocon.GetFloat("humidity.transpiration");
    config.evaporation  = hocon.GetInt("humidity.evaporation");
    
    //Circulation
    config.exchangeCoefficient  = hocon.GetFloat("circulation.exchange_coefficient");
    config.circulationOctaves  = hocon.GetInt("circulation.circulation_octaves");
    config.temperatureInfluence  = hocon.GetFloat("circulation.temperature_influence");
    config.circulationDecline  = hocon.GetInt("circulation.circulation_decline");

        return config;
    }
}
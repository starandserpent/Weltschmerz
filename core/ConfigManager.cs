using System;
using System.IO;
using Akka.Configuration;
using AkkaConfig = Akka.Configuration.Config;
public class ConfigManager{
    public static readonly string CONFIG_NAME = "config.conf";
    public static readonly string BASE_DIRECTORY = AppDomain.CurrentDomain.BaseDirectory;
    public static readonly string BASE_CONFIG_DIRECTORY_PATH = GetConfigDirectoryPath("*"+CONFIG_NAME, BASE_DIRECTORY);
    public static readonly string BASE_CONFIG_FILE_PATH = GetConfigFilePath("*"+CONFIG_NAME, BASE_CONFIG_DIRECTORY_PATH);
    public static Config GetConfig(){
       return GetConfig(BASE_CONFIG_FILE_PATH);
    }
    
    public static Config GetConfig(string path){

        Config config = new Config();

        AkkaConfig hocon = ConfigurationFactory.ParseString(File.ReadAllText(path));
        
        //Map
        config.map.seed = hocon.GetInt("map.seed");
        config.map.latitude = hocon.GetInt("map.latitude");
        config.map.longitude = hocon.GetInt("map.longitude");

        //Noise
        config.noise.frequency = hocon.GetFloat("noise.frequency");
        config.noise.octaves = hocon.GetInt("noise.octaves");
        config.noise.max_elevation = hocon.GetInt("noise.max_elevation");
        config.noise.min_elevation = hocon.GetInt("noise.min_elevation");

        //Temperature
        config.temperature.max_temperature  = hocon.GetInt("temperature.max_temperature");
        config.temperature.min_temperature  = hocon.GetInt("temperature.min_temperature");
        config.temperature.temperature_decrease  = hocon.GetFloat("temperature.temperature_decrease");

        //Precipitation
        config.precipitation.circulation_intensity  = hocon.GetFloat("precipitation.circulation_intensity");
        config.precipitation.precipitation_intensity  = hocon.GetFloat("precipitation.precipitation_intensity");
        config.precipitation.max_precipitation  = hocon.GetInt("precipitation.max_precipitation");

        //Humidity
        config.humidity.transpiration  = hocon.GetFloat("humidity.transpiration");
        config.humidity.evaporation  = hocon.GetFloat("humidity.evaporation");
    
        //Circulation
        config.circulation.wind_intensity  = hocon.GetFloat("circulation.wind_intensity");
        config.circulation.wind_range  = hocon.GetInt("circulation.wind_range");
        config.circulation.pressure_at_sea_level  = hocon.GetInt("circulation.pressure_at_sea_level");

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

    public static string GetConfigDirectoryPath(string fileName, string path){
        DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo(path);
        FileInfo[] filesInDir = hdDirectoryInWhichToSearch.GetFiles(fileName, SearchOption.AllDirectories);

        string fullName = "";
        foreach (FileInfo foundFile in filesInDir)
        {
            fullName = foundFile.DirectoryName;
        }
        
        return fullName;
    }
}
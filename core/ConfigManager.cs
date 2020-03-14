using System;
using System.IO;

/// <summary>
/// Class used for parsing a hocon or json config file to Weltschmerz config class
/// </summary>
public class ConfigManager {

    /// <summary>
    /// Default name of config file
    /// Weltschmerz will search for this file when initialized
    /// </summary>
    public static readonly string CONFIG_FILE_NAME = "config.conf";

    /// <summary>
    /// Path to base directory where weltschmerz is located
    /// </summary>
    public static readonly string BASE_DIRECTORY = AppDomain.CurrentDomain.BaseDirectory;

    /// <summary>
    /// Path to base directory where config file is located
    /// </summary>
    public static readonly string BASE_CONFIG_FILE_DIRECTORY_PATH = FindDirectory ("*" + CONFIG_FILE_NAME, BASE_DIRECTORY);

    /// <summary>
    /// Path to config file
    /// </summary>
    public static readonly string BASE_CONFIG_FILE_PATH = FindFile ("*" + CONFIG_FILE_NAME, BASE_CONFIG_FILE_DIRECTORY_PATH);

    /// <summary>
    /// Gets configuration from default file location (<see cref="ConfigManager"/>)
    /// </summary>
    public static Config GetConfig () {
        return GetConfig (BASE_CONFIG_FILE_PATH);
    }

    /// <summary>
    /// Gets configuration from specified file location (<see cref="ConfigManager"/>)
    /// Use absolute path to file
    /// </summary>
    public static Config GetConfig (string path) {

        Config config = new Config ();

        //Parses valuse from hocon or json file to Akka class
        Hocon.Config hocon = Hocon.HoconConfigurationFactory.ParseString (File.ReadAllText (path));

        //Map
        //Copies all map values from hocon or json file to Config class
        config.map.seed = hocon.GetInt ("map.seed");
        config.map.latitude = hocon.GetInt ("map.latitude");
        config.map.longitude = hocon.GetInt ("map.longitude");

        //Elevation
        //Copies all elevation values from hocon or json file to Config class
        config.elevation.frequency = hocon.GetFloat ("elevation.frequency");
        config.elevation.octaves = hocon.GetInt ("elevation.octaves");
        config.elevation.max_elevation = hocon.GetInt ("elevation.max_elevation");
        config.elevation.min_elevation = hocon.GetInt ("elevation.min_elevation");
        config.elevation.water_level = hocon.GetInt ("elevation.water_level");

        //Temperature
        //Copies all temperature values from hocon or json file to Config class
        config.temperature.max_temperature = hocon.GetInt ("temperature.max_temperature");
        config.temperature.min_temperature = hocon.GetInt ("temperature.min_temperature");

        //Precipitation
        //Copies precipitation map values from hocon or json file to Config class
        config.precipitation.circulation_intensity = hocon.GetFloat ("precipitation.circulation_intensity");
        config.precipitation.precipitation_intensity = hocon.GetFloat ("precipitation.precipitation_intensity");
        config.precipitation.max_precipitation = hocon.GetInt ("precipitation.max_precipitation");

        //Humidity
        //Copies all humidity values from hocon or json file to Config class
        config.humidity.transpiration = hocon.GetFloat ("humidity.transpiration");
        config.humidity.evaporation = hocon.GetFloat ("humidity.evaporation");

        //Circulation
        //Copies all circulation values from hocon or json file to Config class
        config.circulation.wind_intensity = hocon.GetFloat ("circulation.wind_intensity");
        config.circulation.wind_range = hocon.GetInt ("circulation.wind_range");
        config.circulation.pressure_at_sea_level = hocon.GetInt ("circulation.pressure_at_sea_level");

        return config;
    }

    /// <summary>
    /// Finds an absolute path to a file with specified name in specified directory
    /// Returns an absolute path of the file as string
    /// </summary>
    public static string FindFile (string fileName, string baseDirectory) {
        DirectoryInfo directory = new DirectoryInfo (baseDirectory);
        FileInfo[] filesInDir = directory.GetFiles (fileName, SearchOption.AllDirectories);

        foreach (FileInfo foundFile in filesInDir) {
            return foundFile.FullName;
        }

        return "";
    }

    /// <summary>
    /// Finds an absolute path to a directory with specified name in base directory
    /// Returns an absolute path of the directory as string
    /// </summary>
    public static string FindDirectory (string directoryName, string baseDirectory) {
        DirectoryInfo directory = new DirectoryInfo (baseDirectory);
        FileInfo[] filesInDir = directory.GetFiles (directoryName, SearchOption.AllDirectories);

        foreach (FileInfo foundFile in filesInDir) {
            return foundFile.Directory.FullName;
        }

        return BASE_DIRECTORY;
    }
}
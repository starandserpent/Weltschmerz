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
        
/*        config.seed = SEED;
        config.avgTerrain = AVERAGE_TERRAIN_HIGHT;
        config.frequency = FREQUENCY;
        config.latitude = LATITUDE;
        config.longitude = LONGITUDE;
        config.maxElevation = MAX_ELEVATION;
        config.terrainMP = TERRAIN_MULTIPLAIER;
        */

        return config;
    }
}
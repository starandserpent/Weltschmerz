using System.IO;
using Akka.Configuration;
using AkkaConfig = Akka.Configuration.Config;
public class ConfigManager{
    public static Config GetConfig(string path){
        
        Config config = new Config();

        AkkaConfig hocon = ConfigurationFactory.ParseString(File.ReadAllText(path));
        
        
        return config;
    }
}
public class Weltschmerz
{
    private volatile Noise noise; 
    private Config config;
    private const string PATH = "./config/config.conf";

    public Weltschmerz() : this (ConfigManager.GetConfig(PATH)){}

    public Weltschmerz(Config config)
    {
        noise = new Noise();
        this.config = config;
    }

    private void SetConfig(){

    }

    public double GetElevation(int posX, int posY)
    {
        return noise.getNoise(posX, posY);
    }

    public int GetMaxElevation(){
        return noise.GetMaxElevation();
    }
}
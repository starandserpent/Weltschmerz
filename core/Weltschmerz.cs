using System;
public class Weltschmerz
{
    private volatile Noise noise; 
    private Config config;
    public Weltschmerz() : this (ConfigManager.GetConfig()){}

    public Weltschmerz(Config config)
    {
        noise = new Noise(config);
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
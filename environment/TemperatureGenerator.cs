public abstract class TemperatureGenerator : Generator {
    public double EquatorPosition { get; set; }

    public double LapseRate { get; protected set; }

    public TemperatureGenerator (Weltschmerz weltschmerz, Config config) : base(weltschmerz, config) {}
    public abstract double GetTemperatureAtSeaLevel (int posY);
    public abstract double GetTemperature (int posY, double elevation);
    public abstract double GetEquatorDistance (int posY);
}
using System.Numerics;
using System;
public class Circulation : IConfigurable{
    private Noise noise;
    private Temperature equator;
    private int latitude;
    private int longitude;
    private double maxElevation;
    private float circulationDecline;
    private double temperatureInfluence;
    private float exchangeCoefficient;
    private int octaves;
    public Circulation(Config config, Noise noise, Temperature temperature){
        Configure(config);
        this.noise = noise;
        this.equator = temperature;
    }

    public void Configure(Config config){
        this.latitude = config.latitude;
        this.longitude = config.longitude;
        this.maxElevation = config.maxElevation;
        this.circulationDecline = config.circulationDecline;
        this.temperatureInfluence = config.circulationIntensity;
        this.exchangeCoefficient = config.exchangeCoefficient;
        this.octaves = config.circulationOctaves;
    }

   public Vector2 GetAirFlow(int posX, int posY) {
        Vector4 airExchange = CalculateAirExchange(posX, posY) * exchangeCoefficient;

        airExchange = Vector4.Clamp(airExchange, new Vector4(-1F, -1F, -1F, -1F), new Vector4(1F, 1F, 1F, 1F));

        float x = airExchange.X;
        float y = airExchange.Y;

        x += (float) (1 / Math.Sqrt(2)) * airExchange.Z;
        y += (float) (1 / Math.Sqrt(2)) * airExchange.Z;

        x += (float) (1 / Math.Sqrt(2)) * airExchange.W;
        y += (float) -(1 / Math.Sqrt(2)) * airExchange.W;

        return ApplyCoriolisEffect(posY, new Vector2(x, y));
    }

    private Vector4 CalculateAirExchange(int posX, int posY) {
        float x = 0;
        float y = 0;
        float z = 0;
        float w = 0;
        float range = 0.0f;
        float intensity = 1.0f;

        for (int octave = 0; octave < octaves; octave++) {
            Vector4 delta = CalculateDensityDelta(posX, posY, (int) (Math.Pow(octave, 2)));
            x += delta.X * intensity;
            y += delta.Y * intensity;
            z += delta.Z * intensity;
            w += delta.W * intensity;

            range += intensity;
            intensity *= circulationDecline;
        }

        return new Vector4(x / range, y / range, z / range, w / range);
    }

    private Vector4 CalculateDensityDelta(int posX, int posY, int distance) {

        int posXPositive = Math.Min(posX + distance, longitude - 1);
        int posXNegative = Math.Max(posX - distance, 0);
        int posYPositive = Math.Min(posY + distance, latitude - 1);
        int posYNegative = Math.Max(posY - distance, 0);

        double xNegativeDensity = CalculateDensity(posXNegative, posY);
        double xPositiveDensity;
        double yNegativeDensity;
        double yPositiveDensity;
        double zPositiveDensity;
        double zNegativeDensity;
        double wPositiveDensity;
        double wNegativeDensity;

        // west - east
        if(posXNegative == posXPositive){
            xPositiveDensity = xNegativeDensity;
        }else{
            xPositiveDensity = CalculateDensity(posXPositive, posY);
        }

        double x = xNegativeDensity - xPositiveDensity;

        // north - south
        if (posYNegative == posY && posX == xNegativeDensity) {
            yNegativeDensity = xNegativeDensity;
        }else if(posYNegative == posY && posX == xPositiveDensity){
            yNegativeDensity = xPositiveDensity;
        }else{
            yNegativeDensity = CalculateDensity(posX, posYNegative);
        }

        if (posYPositive == posY && posX == xNegativeDensity) {
            yPositiveDensity = xNegativeDensity;
        }else if(posYPositive == posY && posX == xPositiveDensity){
            yPositiveDensity = xPositiveDensity;
        }else {
            yPositiveDensity = CalculateDensity(posX, posYPositive);
        }

        double y = yNegativeDensity - yPositiveDensity;

        // south-west - north-east
        if(posX == posXNegative){
            zNegativeDensity = yNegativeDensity;
        }else if(posY == posYNegative){
            zNegativeDensity = xNegativeDensity;
        }else{
            zNegativeDensity = CalculateDensity(posXNegative, posYNegative);
        }

        if(posX == posXPositive){
            zPositiveDensity = yPositiveDensity;
        }else if(posY == posYPositive){
            zPositiveDensity = xPositiveDensity;
        }else{
            zPositiveDensity = CalculateDensity(posXPositive, posYPositive);
        }

        double z = zNegativeDensity - zPositiveDensity;

        // north-west - south-east
        if(posX == posXNegative){
            wNegativeDensity = yPositiveDensity;
        }else if(posY == posYPositive){
            wNegativeDensity = xNegativeDensity;
        }else{
            wNegativeDensity = CalculateDensity(posXNegative, posYPositive);
        }

        if(posY == posYNegative){
            wPositiveDensity = xPositiveDensity;
        }else if(posX == posXPositive){
            wPositiveDensity = yNegativeDensity;
        }else{
            wPositiveDensity = CalculateDensity(posXPositive, posYNegative);
        }

        double w = wNegativeDensity - wPositiveDensity;

        return new Vector4((float)x, (float)y, (float)z, (float)w);
    }

    public double CalculateDensity(int posX, int posY) {
        double density = CalculateBaseDensity(posY);
        double elevation = noise.getNoise(posX, posY)/maxElevation;
        double temperature = equator.GetTemperature(posY, elevation);
        return (density * (1.0 - temperatureInfluence)) + ((1.0 - temperature) * temperatureInfluence);
    }

    private double CalculateBaseDensity(int posY) {
        double verticallity = Utils.ToUnsignedRange(equator.GetEquatorDistance(posY));
        return Utils.ToUnsignedRange(Math.Cos(verticallity * 3 * (Math.PI * 2)));
    }

    private Vector2 ApplyCoriolisEffect(int posY, Vector2 airFlow) {
        float coriolisLatitude = (float) posY / latitude;
        double equatorPosition = equator.GetEquatorPosition();
        double direction = Math.Sign(coriolisLatitude - equatorPosition);
        Vector4 matrix = Utils.GetRotation((Math.PI / 2) * direction * airFlow.Length());
        float x = (matrix.X * airFlow.X) + (matrix.Z * airFlow.X);
        float y = (matrix.Y * airFlow.Y) + (matrix.W * airFlow.Y);
        return new Vector2(x, y);
    }
}
using System.Numerics;
using System;
public class Circulation : CirculationGenerator{
    private Weltschmerz weltschmerz;

    //N*m/(mol*K)
    private double gasConstant = 8.31432;
    //Pa
    private double pressureAtSeaLevel = 101325;

    //kg/mol
    private double molarMass = 0.0289644;

    //m/s^2
    private double gravitionalAcceleration = 9.80665;

    private double temperatureDecrease = 0.0065;

    private double increment;

    public Circulation(Config config, Weltschmerz weltschmerz) : base(config){
        this.weltschmerz = weltschmerz;
        increment = (gravitionalAcceleration * molarMass)/(temperatureDecrease * gasConstant);
    }

   public override Vector2 GetAirFlow(int posX, int posY) {
        Vector4 airExchange = CalculateAirExchange(posX, posY) * config.circulation.exchange_coefficient;

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
        float x = 1;
        float y = 1;
        float z = 1;
        float w = 1;
        float range = 0.0f;
        float intensity = 1.0f;

        for (int octave = 0; octave < config.circulation.circulation_octaves; octave++) {
            Vector4 delta = CalculateDensityDelta(posX, posY, (int) (Math.Pow(octave, 2)));
            x += delta.X * intensity;
            y += delta.Y * intensity;
            z += delta.Z * intensity;
            w += delta.W * intensity;

            range += intensity;
            intensity *= config.circulation.circulation_decline;
        }
        
        return new Vector4(x / range, y / range, z / range, w / range);
    }

    private Vector4 CalculateDensityDelta(int posX, int posY, int distance) {

        int posXPositive = Math.Min(posX + distance, config.map.longitude - 1);
        int posXNegative = Math.Max(posX - distance, 0);
        int posYPositive = Math.Min(posY + distance, config.map.latitude - 1);
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
        double elevation = weltschmerz.NoiseGenerator.GetNoise(posX, posY);
        double temperature = weltschmerz.TemperatureGenerator.GetTemperature(posY, elevation);
        double density = CalculateBaseDensity(posY) * pressureAtSeaLevel;
        temperature += 273.15;
        density = density * Math.Pow(1 - (temperatureDecrease/temperature) * elevation, increment);
        return density;
    }

    private double CalculateBaseDensity(int posY) {
        double verticallity = (weltschmerz.TemperatureGenerator.GetEquatorDistance(posY)/weltschmerz.TemperatureGenerator.EquatorPosition) * 3;
        return Math.Cos(verticallity * 3) + 1;    
    }

    private Vector2 ApplyCoriolisEffect(int posY, Vector2 airFlow) {
        float coriolisLatitude = (float) posY / config.map.latitude;
        double equatorPosition = weltschmerz.TemperatureGenerator.EquatorPosition;
        double direction = Math.Sign(coriolisLatitude - equatorPosition);
        Vector4 matrix = WeltschmerzUtils.GetRotation((Math.PI / 2) * direction * airFlow.Length());
        float x = (matrix.X * airFlow.X) + (matrix.Z * airFlow.X);
        float y = (matrix.Y * airFlow.Y) + (matrix.W * airFlow.Y);

        return new Vector2(x, y);
    }
}
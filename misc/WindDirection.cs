using System;
public struct WindDirection{
    public double x {get; set;}
    public double y {get; set;}
    public double z {get; set;}
    public double w {get; set;}
    public double getLength() {
        return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2) + Math.Pow(w, 2));
    }
}
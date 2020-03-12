public abstract class NoiseGenerator {
    protected Config config;

    public NoiseGenerator (Config config) {
        this.config = config;
    }

    public abstract double GetNoise (int posX, int posY);
}
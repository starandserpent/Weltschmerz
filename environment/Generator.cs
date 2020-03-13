/// <summary>
/// Elemental class for every generator containing default variables and 
/// functions for configurable generators
/// </summary>
public abstract class Generator {

    //Current config used in generator
    protected Config config;

    //Reference to main Weltschmerz class used for accessing other generators
    protected Weltschmerz weltschmerz;

    public Generator (Weltschmerz weltschmerz, Config config) {
        this.config = config;
        this.weltschmerz = weltschmerz;
        Update ();
    }

    /// <summary>
    /// Used for precalculating variables which depend on values from <see cref="Config"/>
    /// </summary>
    public abstract void Update ();

    /// <summary>
    /// Change config class used in Weltschmerz
    /// </summary>
    public abstract void ChangeConfig (Config config);
}
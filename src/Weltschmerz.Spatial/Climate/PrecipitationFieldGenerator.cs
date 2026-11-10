using Weltschmerz.Spatial.Atlas;

namespace Weltschmerz.Spatial.Climate;

internal static class PrecipitationFieldGenerator {
    private const ulong PrecipitationSalt = 0x5052454349505631UL;

    internal static double SampleNoise(EnvironmentalAtlasDefinition definition, double worldX, double worldZ) =>
        EnvironmentalValueNoise.Sample(definition.WorldSeed, PrecipitationSalt, worldX, worldZ,
            definition.Profile.PrecipitationNoiseSpacingMetres);
}

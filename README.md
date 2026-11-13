# Weltschmerz
![Example of earth generated in World map generator by Weltschmerz](https://github.com/starandserpent/World-map-generator/blob/dev/screenshots/earthbiomes.png)

## Table of Contents

* [About the Project](#about-the-project)
* [Examples](#examples)
* [Getting Started](#getting-started)
  * [Prerequisites](#prerequisites)
  * [Installation](#installation)
    * [Git submodule](#git-submodule)
    * [DLL Library](#dll-library)
* [Usage](#usage)
* [Documentation](#documentation)
* [License](#license)
* [Contact](#contact)
  * [Follow us](#follow-us)


## About the project

Weltschmerz is a library for simulating terrain, moisture, temperature, wind, and precipitation based on real environmental principles. The values Weltschmerz outputs can be used, for example, for generating realistic looking maps or terrain. Weltschmerz is primarily aimed at games and does not produce scientifically accurate data.

Weltschmerz is used in [Foreman](https://github.com/starandserpent/Foreman) to generate 3D voxel terrain and in [World map generator](https://github.com/starandserpent/World-map-generator) to generate  realistic looking maps.

## Modern environmental-atlas API

`src/Weltschmerz.Spatial` is a separate immutable `.NET 10` library for generating a complete terrain-informed environmental atlas. Its main operation accepts an `EnvironmentalAtlasDefinition` and the consumer's complete row-major elevation raster. Weltschmerz does not generate replacement terrain, call Goxlap, read terrain files, or search runtime configuration. The project does not depend on the historical HOCON or FastNoise packages.

The fixed `EnvironmentalAtlasProfile.TemperateAtlasV1` profile includes long-term temperature normals in Celsius, atmospheric pressure in Pascals, circulation X/Z, humidity and surface moisture in normalized 0–1 units, annual precipitation in millimetres/year, supplied terrain elevation in metres, and a stable biome ID. Its deterministic seeded integer-hash value noise and fixed west-to-east moisture-transport pass use the complete supplied topography. Oceans contribute moisture and rising terrain affects precipitation; this is a bounded, reproducible approximation, not a scientifically exact atmospheric simulation or changing weather system.

The initial land-biome identities are `TemperateForest` (1) and `Meadow` (2). Forest classification uses the profile's temperature interval and annual precipitation threshold. Environmental data is generated as one complete operation and exposed read-only; the generator does not retain mutable configuration or lazily generate missing cells. Dark Lantern uses this API during new-world creation, then persists and verifies the resulting atlas. Gameplay population reads retained biome identities and does not call this generator. Its separate `DarkLantern.Game/World/Climate` system continues to own dynamic seasons and weather.

The historical finite-map generator remains available to its existing consumers. Its mutable configuration and noise dependencies are independent of the modern spatial-atlas project.

### Generating an atlas

Reference `src/Weltschmerz.Spatial/Weltschmerz.Spatial.csproj` from the world-creation project and pass the complete row-major elevation raster sampled by your terrain provider. The index is `z * width + x`; each value is the terrain elevation in metres at that geographical cell's center. Weltschmerz returns one complete immutable atlas and does not load terrain or generate missing cells.

```csharp
using Weltschmerz.Spatial.Atlas;

var bounds = new EnvironmentalAtlasBounds(-65_536, -65_536, 256, 256, 512);
var definition = new EnvironmentalAtlasDefinition(
    worldSeed,
    worldGenerationVersion,
    terrainProviderIdentity,
    terrainProfileIdentity,
    EnvironmentalAtlasProfile.TemperateAtlasV1,
    bounds,
    oceanElevationMetres);
var atlas = new EnvironmentalAtlasGenerator().Generate(definition, terrainElevations, cancellationToken);
var biome = atlas.GetCell(x, z).BiomeId;
```

`terrainElevations` must contain exactly `width * height` finite values. Atlas coordinates are horizontal X/Z; the consumer supplies its authoritative terrain elevation rather than asking Weltschmerz to invent replacement geometry.

## Examples

Foreman takes elevation from Weltschmerz and generates 3D voxel world based on this elevation.

Foreman example: https://www.youtube.com/watch?v=lAZFasyDf7Y

* World map generator generates biomes from environmental variables.

![World map generator example](https://github.com/starandserpent/World-map-generator/blob/dev/screenshots/world.png)

## Getting Started

The modern atlas API targets .NET 10 and has no HOCON, FastNoise, or runtime configuration-file dependency. The historical setup instructions below apply only to consumers of the finite-map API.

### Modern API prerequisites

Use the .NET 10 SDK and reference `src/Weltschmerz.Spatial/Weltschmerz.Spatial.csproj` from the project that performs offline atlas generation.

### Historical API prerequisites

The legacy `Weltschmerz.csproj` uses its historical HOCON configuration and FastNoise dependency. Its old configuration-file and installation guidance below is retained for those consumers only; neither is required by `Weltschmerz.Spatial`.

#### Legacy installation
A step by step series how to install library into your project
1) Select or make your .NET project

2) Include [C# Hocon library](https://www.nuget.org/packages/Hocon.Configuration/) in your .csproj file. (File is located in your .NET project)

```
  <ItemGroup>
    <PackageReference Include="Hocon.Configuration">
      <Version>2.0.3</Version>
    </PackageReference>
  </ItemGroup>
```

Now you have 2 options how to include Weltschmerz in your project.

#### Git submodule
3) Go to your project folder
4) Initialize git in your repository
```git
git init
```
5) Add Weltschmerz as git submodule
```git
git submodule add git@github.com:starandserpent/Weltschmerz.git --recusive
```
6) Include all .cs files from Weltschmerz in your project

#### DLL Library
3) Open Weltschmerz in your IDE
4) Build solution
5) Open your project
6) Add Weltschmerz library as reference into your project


#### Legacy API usage
You can access all environmental variables from *Weltschmerz* class
```csharp
    Weltschmerz weltschmerz = new Weltschmerz();
    double elevation = weltschmerz.GetElevation(0, 0);
```

If you want to replace any generator in weltschmerz by your own, use abstract class
```csharp
    weltschmerz.ElevationGenerator = new Elevation();
```

Default config file is named "config.conf", use *ConfigManager* to load your custom config files.
```csharp
    string path = "yourpath/to/file"
    Config config = ConfigManager.GetConfig(path);
```
For further implementation as library in 3D game see [Foreman](https://github.com/starandserpent/Foreman) or for map generation see [World-map-generator](https://github.com/starandserpent/World-map-generator)

## Documentation
You can find documetation on [github wiki](https://github.com/starandserpent/Weltschmerz/wiki)

## Licence
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details

## Contact us
* Discord: https://discord.starandserpent.com/

### Follow us
* Website: https://www.ritualsoftheold.com/
* IndieDB: https://www.indiedb.com/games/rituals-of-the-old
* Subscribe: https://www.youtube.com/c/Ritualsoftheold

This library is also used in [Rituals  of the old](https://www.ritualsoftheold.com/).

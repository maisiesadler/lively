# Dependency Tree

[![Release Nuget Package](https://github.com/maisiesadler/deptree/actions/workflows/release.yml/badge.svg)](https://github.com/maisiesadler/deptree/actions/workflows/release.yml)
[![Generate Diagrams](https://github.com/maisiesadler/deptree/actions/workflows/generate-diagrams.yml/badge.svg)](https://github.com/maisiesadler/deptree/actions/workflows/generate-diagrams.yml)

Dependency Tree will load an assembly then, starting with the root type, recursively read the constructor parameters to create a tree of dependencies.

By default it uses the Startup file to load and resolve which type is registered to an interface.

[Example ouput](./DependencyTree.md) for this repository:

<img src="http://yuml.me/diagram/scruffy/class/[DependencyTree]-&gt;[DependencyTreeConfig], [DependencyTreeConfig]-&gt;[Assembly], [DependencyTreeConfig]-&gt;[IConfiguration], [DependencyTreeConfig]-&gt;[HashSet`1], [DependencyTreeConfig]-&gt;[String]" />

## Using the github action

The GitHub action is defined [here](./acton.yml) and  uses the [Dockerfile](./Dockerfile) in the root of the project.

| Name | Environment Variable | CLI setting | | Required |
| -- | -- | -- | -- | -- |
| Assembly Location | `ASSEMBLY_LOCATION` | `-a` `--assembly` | The location of the assembly to read | Yes |
| Root types | `ROOT_TYPES` | `-t` `--root-types` | The root type to use for the dependency tree, multiple values can be used as a csv input | Yes |
| Skip types | `SKIP_TYPES` | `-s` `--skip-types` | Types to skip, multiple values can be used as a csv input | No |
| Assembly Config Location | `ASSEMBLY_CONFIG_LOCATION` | `--assembly-config` | The location of the configuration file required to build IConfiguration for Startup | No |
| [Interface Resolver Type](#interface-resolver-type) | `INTERFACE_RESOLVER` | `-i` `--interface-resolver` | Method for resolving interfaces, Allowed Values: None, Startup. Default: Startup. | No |
| Startup Name | `STARTUP_NAME` | `--startup-name` | Startup Type Name or FullName. Default: `Startup`. | No |
| [Output Format](#output-format) | `OUTPUT_FORMAT` | `--output-format` | Format to print out the result. Allowed values: `debug`, `yumlmd`, `yuml`, `mermaid`. Default: `yuml`. | No |
| [Application Config Location](#application-config) | `APPLICATION_CONFIG_LOCATION` | `-c` `--config` | The location of application config file | No |

### Application config

There is support for passing in a config file for multiple `Skip` or `Generate` (root) types.

[Example](./applicationconfig.json)

### Interface Resolver Type

If `Interface Resolver` is `Startup` (default) then the application will look for a class named Startup.

If the Startup file has an IConfiguration constructor then set the `Assembly Config Location` to the path of an example config file that has enough data in it to build the configuration.

If there is no startup then set `Interface Resolver` to `None`.

The Startup file name can be overriden to either the type Name or FullName. E.g. `TestStartup` or `DepTree.MultipleApplications.Startup`.

### Example applications

#### Pokedex - dotnet5 web api

- Needed to publish a single file application `dotnet publish -r linux-x64 -p:PublishSingleFile=true -p:UseAppHost=true --self-contained`
- Need to add RuntimeIdentifiers [here](https://github.com/maisiesadler/pokedex/blob/main/src/Pokedex/Pokedex.csproj#L5).

[Example output](https://github.com/maisiesadler/pokedex/blob/main/DependencyTree.md)

<img src="http://yuml.me/diagram/scruffy/class/[PokemonController]-&gt;[BasicPokemonInformationRetriever], [PokemonController]-&gt;[TranslatedPokemonInformationRetriever], [PokemonController]-&gt;[ILogger`1], [BasicPokemonInformationRetriever]-&gt;[IPokemonQuery|PokemonQuery], [IPokemonQuery]-2&gt;[ICache`1], [IPokemonQuery]-2&gt;[IPokeApiClient], [TranslatedPokemonInformationRetriever]-&gt;[IPokemonQuery|PokemonQuery], [TranslatedPokemonInformationRetriever]-&gt;[ITranslationQuery|TranslationQuery], [ITranslationQuery]-&gt;[ICache`1], [ITranslationQuery]-&gt;[IFunTranslationsApiClient], [ITranslationQuery]-&gt;[ILogger`1]" />

[Example workflow](https://github.com/maisiesadler/pokedex/blob/main/.github/workflows/dependencytree.yml)

#### Endpoints - doesn't use Startup

[Example output](https://github.com/maisiesadler/Endpoints/blob/master/Dependencies.md)

<img src="http://yuml.me/diagram/scruffy/class/[MyModelRetriever]-&gt;[IDbThing]" />

[Example workflow action](https://github.com/maisiesadler/Endpoints/blob/master/.github/workflows/dependencytree.yml)

## Output formats

### UML

There are 2 output formats using [yUML](https://yuml.me/), `yuml` and `yumlmd`.

- `yuml` - creates a yUML diagram that can be parsed by [this](https://marketplace.visualstudio.com/items?itemName=JaimeOlivares.yuml) vscode extension which produces a SVG
- `yumlmd` - creates a URL that is dynamically created into an image by yuml.me which can then be displayed in a html document

## Mermaid

[Mermaid](https://mermaid-js.github.io/) is a javascript based diagramming and charting tool.
There are a few vscode extensions that can be used to view the diagrams locally and a [chrome](https://github.com/BackMarket/github-mermaid-extension) extension that can be added to view the diagrams in GitHub.

- `mermaid`

## Nuget

- [DepTree](https://www.nuget.org/packages/DepTree)
- [DepTree.Diagrams](https://www.nuget.org/packages/DepTree.Diagrams)

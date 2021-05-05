# Dependency Tree

Dependency Tree will load an assembly then, starting with the root type, recursively read the constructor parameters to create a tree of dependencies.

By default it uses the Startup file to load and resolve which type is registered to an interface.

## Using the github action

The GitHub action uses the [Dockerfile](./Dockerfile) in the root of the project.

| Name | Environment Variable | CLI setting | | Required |
| -- | -- | -- | -- | -- |
| Assembly Location | ASSEMBLY_LOCATION | -a --assembly | The location of the assembly to read | Yes |
| Root types | ROOT_TYPES | -t --root-types | The root type to use for the dependency tree, multiple values can be used as a csv input | Yes |
| Skip types | SKIP_TYPES | -s --skip-types | Types to skip, multiple values can be used as a csv input | No |
| Assembly Config Location | ASSEMBLY_CONFIG_LOCATION | -n --assembly-config | The location of the configuration file required to build IConfiguration for Startup | No |
| Interface Resolver | INTERFACE_RESOLVER | -i --interface-resolver | Method for resolving interfaces, Allowed Values: None, Startup. Default: Startup. | No |
| Application Config Location | APPLICATION_CONFIG_LOCATION | -n --assembly-config | The location of application config file | No |

### Application config

There is support for passing in a config file for multiple `Skip` or `Generate` (root) types.

[Example](./applicationconfig.json)

### Interface resolver

If `Interface Resolver` is `Startup` (default) then the application will look for a class named Startup.

If the Startup file has an IConfiguration constructor then set the `Assembly Config Location` to the path of an example config file that has enough data in it to build the configuration.

If there is no startup then set `Interface Resolver` to `None`.

### Example applications

#### Pokedex - dotnet5 web api

Needed to publish a single file application

`dotnet publish -r linux-x64 -p:PublishSingleFile=true -p:UseAppHost=true --self-contained`

[Example output](https://github.com/maisiesadler/pokedex/blob/main/DependencyTree.md)

<img src="http://yuml.me/diagram/scruffy/class/[PokemonController]-&gt;[BasicPokemonInformationRetriever], [PokemonController]-&gt;[TranslatedPokemonInformationRetriever], [PokemonController]-&gt;[ILogger`1], [BasicPokemonInformationRetriever]-&gt;[IPokemonQuery|PokemonQuery], [IPokemonQuery]-2&gt;[ICache`1], [IPokemonQuery]-2&gt;[IPokeApiClient], [TranslatedPokemonInformationRetriever]-&gt;[IPokemonQuery|PokemonQuery], [TranslatedPokemonInformationRetriever]-&gt;[ITranslationQuery|TranslationQuery], [ITranslationQuery]-&gt;[ICache`1], [ITranslationQuery]-&gt;[IFunTranslationsApiClient], [ITranslationQuery]-&gt;[ILogger`1]" />

[Example workflow](https://github.com/maisiesadler/pokedex/blob/main/.github/workflows/dependencytree.yml)

- Need to add RuntimeIdentifiers [here](https://github.com/maisiesadler/pokedex/blob/main/src/Pokedex/Pokedex.csproj#L5).

#### Endpoints - doesn't use Startup

[Example output](https://github.com/maisiesadler/Endpoints/blob/master/Dependencies.md)

<img src="http://yuml.me/diagram/scruffy/class/[MyModelRetriever]-&gt;[IDbThing]" />

[Example workflow action](https://github.com/maisiesadler/Endpoints/blob/master/.github/workflows/dependencytree.yml)

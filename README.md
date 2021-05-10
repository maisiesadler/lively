# Dependency Tree

[![Release Nuget Package](https://github.com/maisiesadler/deptree/actions/workflows/release.yml/badge.svg)](https://github.com/maisiesadler/deptree/actions/workflows/release.yml)
[![Generate Diagrams](https://github.com/maisiesadler/deptree/actions/workflows/generate-diagrams.yml/badge.svg)](https://github.com/maisiesadler/deptree/actions/workflows/generate-diagrams.yml)
[![Use GitHub Action](https://github.com/maisiesadler/deptree/actions/workflows/github-action-tests.yml/badge.svg)](https://github.com/maisiesadler/deptree/actions/workflows/github-action-tests.yml)

[![DepTree](https://img.shields.io/nuget/v/DepTree)](https://www.nuget.org/packages/DepTree/)
[![DepTree.Diagrams](https://img.shields.io/nuget/v/DepTree.Diagrams)](https://www.nuget.org/packages/DepTree/Diagrams/)

Dependency Tree uses reflection to automatically create a tree of class dependencies for given types in an assembly.
It is available as a [GitHub Action](#using-the-github-action) and a [Nuget](#nuget) package.

By default it uses the Startup file to load and resolve which type is registered to an interface.

There are a few different [output formats](#output-formats) that can be configured, the default is `yumlmd`.

[Example yumlmd ouput](./DependencyTree.md) for this repository:

<img src="http://yuml.me/diagram/scruffy/class/[DependencyTree]-&gt;[DependencyTreeConfig], [DependencyTreeConfig]-&gt;[Assembly], [DependencyTreeConfig]-&gt;[IConfiguration], [DependencyTreeConfig]-&gt;[HashSet`1], [DependencyTreeConfig]-&gt;[String]" />

## Using the github action

Available [here](https://github.com/marketplace/actions/generate-dependency-diagrams).

The GitHub action is defined in [./action.yml](./action.yml) and  uses the [Dockerfile](./Dockerfile) in the root of the project.

Example outputs can be found [here](./example-outputs)

### Example worflow

```
name: Use GitHub Action

on: push

jobs:

  generate_examples:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v2
    - uses: actions/setup-dotnet@v1

    - name: Build project
      run: dotnet build -c Release

    - name: Generate yuml
      id: generate_yuml
      uses: maisiesadler/deptree@v1.0.3-gh-action-test.0
      env:
        ASSEMBLY_LOCATION: './src/DepTree/bin/Release/net5.0/DepTree.dll'
        ROOT_TYPES: 'DepTree.DependencyTree'
        INTERFACE_RESOLVER: None
        OUTPUT_FORMAT: yuml

    - name: Write outputs to file
      shell: bash
      run: |
        echo '${{ steps.generate_yuml.outputs.result }}' > example-outputs/yuml.yum

    - uses: EndBug/add-and-commit@v7
      name: Commit Changes
      with:
        default_author: github_actions
        message: 'Generate diagrams'
```

### Configuration

| Name | Environment Variable | CLI setting | | Required |
| -- | -- | -- | -- | -- |
| Assembly Location | `ASSEMBLY_LOCATION` | `-a` `--assembly` | The location of the assembly to read | Yes |
| Root types | `ROOT_TYPES` | `-t` `--root-types` | The root type to use for the dependency tree, multiple values can be used as a csv input | Yes |
| Skip types | `SKIP_TYPES` | `-s` `--skip-types` | Types to not include in diagram, multiple values can be used as a csv input | No |
| Assembly Config Location | `ASSEMBLY_CONFIG_LOCATION` | `--assembly-config` | The location of the configuration file required to build IConfiguration for Startup | No |
| [Interface Resolver Type](#interface-resolver-type) | `INTERFACE_RESOLVER` | `-i` `--interface-resolver` | Method for resolving interfaces, Allowed Values: None, Startup. Default: Startup. | No |
| Startup Name | `STARTUP_NAME` | `--startup-name` | Startup Type Name or FullName. Default: `Startup`. | No |
| [Output Format](#output-format) | `OUTPUT_FORMAT` | `--output-format` | Format to print out the result. Allowed values: `debug`, `yumlmd`, `yuml`, `mermaid`, `mermaidmd`, `plantuml`. Default: `yumlmd`. | No |
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

`yumlmd` is the default.

### UML

### yUML

There are 2 output formats using [yUML](https://yuml.me/), `yuml` and `yumlmd`.

- `yuml` - creates a yUML diagram that can be parsed by [this](https://marketplace.visualstudio.com/items?itemName=JaimeOlivares.yuml) vscode extension which produces a SVG
- `yumlmd` - creates a URL that is dynamically created into an image by yuml.me which can then be displayed in a html document

#### PlantUML

[PlantUML](https://plantuml.com/class-diagram)

- `plantuml`

### Mermaid

[Mermaid](https://mermaid-js.github.io/) is a javascript based diagramming and charting tool.
There are a few vscode extensions that can be used to view the diagrams locally and a [chrome](https://github.com/BackMarket/github-mermaid-extension) extension that can be added to view the diagrams in GitHub.

- `mermaid` - mermaid diagram
- `mermaidmd` - mermaid diagram with ```mermaid ``` syntax that can be rendered by vscode/chrome extension in a markdown file

## Nuget

- [DepTree](https://www.nuget.org/packages/DepTree)
- [DepTree.Diagrams](https://www.nuget.org/packages/DepTree.Diagrams)

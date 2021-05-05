dotnet publish ./src/DepTree.Console/DepTree.Console.csproj -c Release -o out --no-self-contained

ASSEMBLY_LOCATION=/Users/maisiesadler/repos/endpoints/src/Endpoints.Api/bin/Release/net5.0/osx.10.12-x64/Endpoints.Api.dll
ROOT_TYPES=Pokedex.Controllers.PokemonController

ASSEMBLY_LOCATION=$ASSEMBLY_LOCATION \
  APPLICATION_CONFIG_LOCATION=$APPLICATION_CONFIG_LOCATION \
  ROOT_TYPES=$ROOT_TYPES \
  SKIP_TYPES=$SKIP_TYPES \
  ASSEMBLY_CONFIG_LOCATION=$ASSEMBLY_CONFIG_LOCATION \
  INTERFACE_RESOLVER=$INTERFACE_RESOLVER \
  dotnet out/DepTree.Console.dll

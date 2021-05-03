FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build-env

COPY . ./
RUN dotnet publish ./src/DepTree.Console/DepTree.Console.csproj -c Release -o out --no-self-contained

# Label the container
LABEL maintainer="Maisie Sadler <maisie.sadler>"
LABEL repository="https://github.com/maisiesadler/deptree"
LABEL homepage="https://github.com/maisiesadler/deptree"

LABEL com.github.actions.name="The name of your GitHub Action"
LABEL com.github.actions.description="The description of your GitHub Action."
LABEL com.github.actions.icon="activity"
LABEL com.github.actions.color="orange"

FROM mcr.microsoft.com/dotnet/aspnet:5.0-focal
COPY --from=build-env /out .
COPY --from=build-env ./runindocker.sh ./runindocker.sh
ENTRYPOINT [ "./runindocker.sh" ]

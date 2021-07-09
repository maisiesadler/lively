FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build-env

COPY . ./
RUN dotnet publish ./src/Lively.Console/Lively.Console.csproj -c Release -o out --no-self-contained

# Label the container
LABEL maintainer="Maisie Sadler <maisie.sadler>"
LABEL repository="https://github.com/maisiesadler/lively"
LABEL homepage="https://github.com/maisiesadler/lively"

LABEL com.github.actions.name="The name of your GitHub Action"
LABEL com.github.actions.description="The description of your GitHub Action."
LABEL com.github.actions.icon="activity"
LABEL com.github.actions.color="orange"

ARG ASSEMBLY_LOCATION
ENV ASSEMBLY_LOCATION=$ASSEMBLY_LOCATION
ARG ASSEMBLY_PATTERN_MATCH
ENV ASSEMBLY_PATTERN_MATCH=$ASSEMBLY_PATTERN_MATCH
ARG APPLICATION_CONFIG_LOCATION
ENV APPLICATION_CONFIG_LOCATION=$APPLICATION_CONFIG_LOCATION
ARG ROOT_TYPE
ENV ROOT_TYPE=$ROOT_TYPE
ARG SKIP_TYPE
ENV SKIP_TYPE=$SKIP_TYPE
ARG ASSEMBLY_CONFIG_LOCATION
ENV ASSEMBLY_CONFIG_LOCATION=$ASSEMBLY_CONFIG_LOCATION
ARG INTERFACE_RESOLVER
ENV INTERFACE_RESOLVER=$INTERFACE_RESOLVER

FROM mcr.microsoft.com/dotnet/aspnet:5.0-focal
COPY --from=build-env /out .
COPY --from=build-env ./runindocker.sh /runindocker.sh
ENTRYPOINT [ "/runindocker.sh" ]

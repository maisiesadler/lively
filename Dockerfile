FROM mcr.microsoft.com/dotnet/aspnet:5.0-focal AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build
WORKDIR /build
COPY deptree.sln .
COPY src/DepTree/DepTree.csproj src/DepTree/
COPY src/DepTree.Console/DepTree.Console.csproj src/DepTree.Console/
COPY src/DepTree.Diagrams/DepTree.Diagrams.csproj src/DepTree.Diagrams/
COPY src/DepTree.Tests/DepTree.Tests.csproj src/DepTree.Tests/
ARG ASPNETCORE_ENVIRONMENT
ARG VERSION=1.0.0
RUN dotnet restore deptree.sln
COPY . .
RUN dotnet build -c Release /property:Version=$VERSION --no-restore && \
  dotnet test src/DepTree.Tests/DepTree.Tests.csproj -c Release --no-restore --no-build && \
  dotnet publish src/DepTree.Console/DepTree.Console.csproj -c Release -o /build/publish -r linux-x64 -p:PublishSingleFile=true --self-contained true --no-restore --no-build

FROM base AS final
WORKDIR /app
ENTRYPOINT ["dotnet", "DepTree.Console.dll"]

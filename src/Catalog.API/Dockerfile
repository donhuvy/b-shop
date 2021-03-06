FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

WORKDIR /src
COPY ["Catalog.API/*.csproj", "Catalog.API/"]
COPY ["BuildingBlocks/Data.Entities/*.csproj", "BuildingBlocks/Data.Entities/"]
COPY ["BuildingBlocks/Data.UnitOfWork.EF/*.csproj", "BuildingBlocks/Data.UnitOfWork.EF/"]
COPY ["BuildingBlocks/Shared/*.csproj", "BuildingBlocks/Shared/"]
COPY ["BuildingBlocks/Shared.Grpc/*.csproj", "BuildingBlocks/Shared.Grpc/"]
COPY ["Protos/", "Protos/"]
RUN dotnet restore "Catalog.API/Catalog.API.csproj"

COPY ["Catalog.API/", "Catalog.API/"]
COPY ["BuildingBlocks/Data.Entities/", "BuildingBlocks/Data.Entities/"]
COPY ["BuildingBlocks/Data.UnitOfWork.EF/", "BuildingBlocks/Data.UnitOfWork.EF/"]
COPY ["BuildingBlocks/Shared/", "BuildingBlocks/Shared/"]
COPY ["BuildingBlocks/Shared.Grpc/", "BuildingBlocks/Shared.Grpc/"]

WORKDIR /src/Catalog.API
RUN dotnet build --no-restore -c Release
RUN dotnet publish --no-build -c Release -o ./publish

FROM base AS final
WORKDIR /app
COPY --from=build ["/src/Catalog.API/publish/", "."]
ENTRYPOINT ["dotnet", "Catalog.API.dll"]

FROM registry.micsun.cn/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM registry.micsun.cn/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["src/EIA.S0.Application/EIA.S0.Application.csproj", "src/EIA.S0.Application/"]
COPY ["src/EIA.S0.Contracts/EIA.S0.Contracts.csproj", "src/EIA.S0.Contracts/"]
COPY ["src/EIA.S0.Domain/EIA.S0.Domain.csproj", "src/EIA.S0.Domain/"]
COPY ["src/EIA.S0.Infrastructure/EIA.S0.Infrastructure.csproj", "src/EIA.S0.Infrastructure/"]
COPY ["src/EIA.S0.Infrastructure.EntityFrameworkCore.PostgreSQL/EIA.S0.Infrastructure.EntityFrameworkCore.PostgreSQL.csproj", "src/EIA.S0.Infrastructure.EntityFrameworkCore.PostgreSQL/"]
COPY ["src/EIA.S0.WebApi/EIA.S0.WebApi.csproj", "src/EIA.S0.WebApi/"]
RUN dotnet restore "./src/EIA.S0.WebApi/EIA.S0.WebApi.csproj"
COPY . .
WORKDIR "/src/src/EIA.S0.WebApi"
RUN dotnet build "./EIA.S0.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EIA.S0.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EIA.S0.WebApi.dll"]
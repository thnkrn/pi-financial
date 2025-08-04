# Use the official .NET 8 runtime image as the base image for the runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Use the official .NET 8 SDK image as the build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ["Pi.SetMarketData.MigrationProxy/Pi.SetMarketData.MigrationProxy.csproj", "Pi.SetMarketData.MigrationProxy/"]
COPY ["Pi.SetMarketData.Application/Pi.SetMarketData.Application.csproj", "Pi.SetMarketData.Application/"]
COPY ["Pi.SetMarketData.Domain/Pi.SetMarketData.Domain.csproj", "Pi.SetMarketData.Domain/"]
COPY ["Pi.SetMarketData.Infrastructure/Pi.SetMarketData.Infrastructure.csproj", "Pi.SetMarketData.Infrastructure/"]
COPY ["nuget.config", ""]
RUN dotnet restore "Pi.SetMarketData.MigrationProxy/Pi.SetMarketData.MigrationProxy.csproj"
COPY . .
WORKDIR "/src/Pi.SetMarketData.MigrationProxy"

# Todo: Should use `Release` mode
# RUN dotnet build "Pi.SetMarketData.MigrationProxy.csproj" -c Release -o /app/build
RUN dotnet build "Pi.SetMarketData.MigrationProxy.csproj" -c Debug -o /app/build

FROM build AS publish

# Todo: Should use `Release` mode
# RUN dotnet publish "Pi.SetMarketData.MigrationProxy.csproj" -c Release -o /app/publish /p:UseAppHost=false
RUN dotnet publish "Pi.SetMarketData.MigrationProxy.csproj" -c Debug -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 5003
ENTRYPOINT ["dotnet", "Pi.SetMarketData.MigrationProxy.dll"]

# Command:
# docker build -t set_migration_proxy -f Pi.SetMarketData.MigrationProxy/Dockerfile .
# Use the official .NET 8 runtime image as the base image for the runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Install tzdata and set the time zone
RUN apt-get update && apt-get install -y tzdata
ENV TZ=Asia/Bangkok
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

# Use the official .NET 8 SDK image as the build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ["Pi.GlobalMarketData.DataMigrationDBWorker/Pi.GlobalMarketData.DataMigrationDBWorker.csproj", "Pi.GlobalMarketData.DataMigrationDBWorker/"]
COPY ["Pi.GlobalMarketData.Application/Pi.GlobalMarketData.Application.csproj", "Pi.GlobalMarketData.Application/"]
COPY ["Pi.GlobalMarketData.Domain/Pi.GlobalMarketData.Domain.csproj", "Pi.GlobalMarketData.Domain/"]
COPY ["Pi.GlobalMarketData.Infrastructure/Pi.GlobalMarketData.Infrastructure.csproj", "Pi.GlobalMarketData.Infrastructure/"]
COPY ["nuget.config", ""]
RUN dotnet restore "Pi.GlobalMarketData.DataMigrationDBWorker/Pi.GlobalMarketData.DataMigrationDBWorker.csproj"
COPY . .
WORKDIR "/src/Pi.GlobalMarketData.DataMigrationDBWorker"

# Use `Release` mode
RUN dotnet build "Pi.GlobalMarketData.DataMigrationDBWorker.csproj" -c Release -o /app/build
# RUN dotnet build "Pi.GlobalMarketData.DataMigrationDBWorker.csproj" -c Debug -o /app/build

FROM build AS publish

# Use `Release` mode
RUN dotnet publish "Pi.GlobalMarketData.DataMigrationDBWorker.csproj" -c Release -o /app/publish /p:UseAppHost=false
# RUN dotnet publish "Pi.GlobalMarketData.DataMigrationDBWorker.csproj" -c Debug -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 5003
ENTRYPOINT ["dotnet", "Pi.GlobalMarketData.DataMigrationDBWorker.dll"]

# Command:
# docker build -t set_data_migration_db_worker -f Pi.GlobalMarketData.DataMigrationDBWorker/Dockerfile .
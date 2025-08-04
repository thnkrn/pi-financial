# Use the official .NET 8 runtime image as the base image for the runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Use the official .NET 8 SDK image as the build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ["Pi.SetMarketDataRealTime.DataServer/Pi.SetMarketDataRealTime.DataServer.csproj", "Pi.SetMarketDataRealTime.DataServer/"]
COPY ["Pi.SetMarketDataRealTime.Application/Pi.SetMarketDataRealTime.Application.csproj", "Pi.SetMarketDataRealTime.Application/"]
COPY ["Pi.SetMarketDataRealTime.Domain/Pi.SetMarketDataRealTime.Domain.csproj", "Pi.SetMarketDataRealTime.Domain/"]
COPY ["Pi.SetMarketDataRealTime.Infrastructure/Pi.SetMarketDataRealTime.Infrastructure.csproj", "Pi.SetMarketDataRealTime.Infrastructure/"]
COPY ["nuget.config", ""]
RUN dotnet restore "Pi.SetMarketDataRealTime.DataServer/Pi.SetMarketDataRealTime.DataServer.csproj"
COPY . .
WORKDIR "/src/Pi.SetMarketDataRealTime.DataServer"

# Todo: Should use `Release` mode
# RUN dotnet build "Pi.SetMarketDataRealTime.DataServer.csproj" -c Release -o /app/build
RUN dotnet build "Pi.SetMarketDataRealTime.DataServer.csproj" -c Debug -o /app/build

FROM build AS publish

# Todo: Should use `Release` mode
# RUN dotnet publish "Pi.SetMarketDataRealTime.DataServer.csproj" -c Release -o /app/publish /p:UseAppHost=false
RUN dotnet publish "Pi.SetMarketDataRealTime.DataServer.csproj" -c Debug -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Pi.SetMarketDataRealTime.DataServer.dll"]

# Command:
# docker build -t set_realtime_data_server_manual_build -f Pi.SetMarketDataRealTime.DataServer/Dockerfile .
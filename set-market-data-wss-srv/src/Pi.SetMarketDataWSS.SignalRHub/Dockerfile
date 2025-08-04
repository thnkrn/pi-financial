# Use the official .NET 8 runtime image as the base image for the runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Use the official .NET 8 SDK image as the build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ["Pi.SetMarketDataWSS.SignalRHub/Pi.SetMarketDataWSS.SignalRHub.csproj", "Pi.SetMarketDataWSS.SignalRHub/"]
COPY ["Pi.SetMarketDataWSS.Application/Pi.SetMarketDataWSS.Application.csproj", "Pi.SetMarketDataWSS.Application/"]
COPY ["Pi.SetMarketDataWSS.Domain/Pi.SetMarketDataWSS.Domain.csproj", "Pi.SetMarketDataWSS.Domain/"]
COPY ["Pi.SetMarketDataWSS.Infrastructure/Pi.SetMarketDataWSS.Infrastructure.csproj", "Pi.SetMarketDataWSS.Infrastructure/"]
COPY ["nuget.config", ""]
RUN dotnet restore "Pi.SetMarketDataWSS.SignalRHub/Pi.SetMarketDataWSS.SignalRHub.csproj"
COPY . .
WORKDIR "/src/Pi.SetMarketDataWSS.SignalRHub"

# Todo: Should use `Release` mode
RUN dotnet build "Pi.SetMarketDataWSS.SignalRHub.csproj" -c Release -o /app/build
# RUN dotnet build "Pi.SetMarketDataWSS.SignalRHub.csproj" -c Debug -o /app/build

FROM build AS publish

# Todo: Should use `Release` mode
RUN dotnet publish "Pi.SetMarketDataWSS.SignalRHub.csproj" -c Release -o /app/publish /p:UseAppHost=false
# RUN dotnet publish "Pi.SetMarketDataWSS.SignalRHub.csproj" -c Debug -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 5002
ENTRYPOINT ["dotnet", "Pi.SetMarketDataWSS.SignalRHub.dll"]

# Command:
# docker build -t set_signalr_hub -f Pi.SetMarketDataWSS.SignalRHub/Dockerfile .
# Use the official .NET 8 runtime image as the base image for the runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Use the official .NET 8 SDK image as the build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ["Pi.GlobalMarketDataWSS.SignalRHub/Pi.GlobalMarketDataWSS.SignalRHub.csproj", "Pi.GlobalMarketDataWSS.SignalRHub/"]
COPY ["Pi.GlobalMarketDataWSS.Application/Pi.GlobalMarketDataWSS.Application.csproj", "Pi.GlobalMarketDataWSS.Application/"]
COPY ["Pi.GlobalMarketDataWSS.Domain/Pi.GlobalMarketDataWSS.Domain.csproj", "Pi.GlobalMarketDataWSS.Domain/"]
COPY ["Pi.GlobalMarketDataWSS.Infrastructure/Pi.GlobalMarketDataWSS.Infrastructure.csproj", "Pi.GlobalMarketDataWSS.Infrastructure/"]
COPY ["nuget.config", ""]
RUN dotnet restore "Pi.GlobalMarketDataWSS.SignalRHub/Pi.GlobalMarketDataWSS.SignalRHub.csproj"
COPY . .
WORKDIR "/src/Pi.GlobalMarketDataWSS.SignalRHub"

# Todo: Should use `Release` mode
RUN dotnet build "Pi.GlobalMarketDataWSS.SignalRHub.csproj" -c Release -o /app/build
# RUN dotnet build "Pi.GlobalMarketDataWSS.SignalRHub.csproj" -c Debug -o /app/build

FROM build AS publish

# Todo: Should use `Release` mode
RUN dotnet publish "Pi.GlobalMarketDataWSS.SignalRHub.csproj" -c Release -o /app/publish /p:UseAppHost=false
# RUN dotnet publish "Pi.GlobalMarketDataWSS.SignalRHub.csproj" -c Debug -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 5002
ENTRYPOINT ["dotnet", "Pi.GlobalMarketDataWSS.SignalRHub.dll"]

# Command:
# docker build -t ge_signalr_hub -f Pi.GlobalMarketDataWSS.SignalRHub/Dockerfile .
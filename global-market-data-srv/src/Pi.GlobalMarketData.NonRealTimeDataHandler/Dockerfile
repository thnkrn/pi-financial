# Use the official .NET 8 runtime image as the base image for the runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Use the official .NET 8 SDK image as the build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ["Pi.GlobalMarketData.NonRealTimeDataHandler/Pi.GlobalMarketData.NonRealTimeDataHandler.csproj", "Pi.GlobalMarketData.NonRealTimeDataHandler/"]
COPY ["Pi.GlobalMarketData.Application/Pi.GlobalMarketData.Application.csproj", "Pi.GlobalMarketData.Application/"]
COPY ["Pi.GlobalMarketData.Domain/Pi.GlobalMarketData.Domain.csproj", "Pi.GlobalMarketData.Domain/"]
COPY ["Pi.GlobalMarketData.Infrastructure/Pi.GlobalMarketData.Infrastructure.csproj", "Pi.GlobalMarketData.Infrastructure/"]
COPY ["nuget.config", ""]
RUN dotnet restore "Pi.GlobalMarketData.NonRealTimeDataHandler/Pi.GlobalMarketData.NonRealTimeDataHandler.csproj"
COPY . .
WORKDIR "/src/Pi.GlobalMarketData.NonRealTimeDataHandler"

# Todo: Should use `Release` mode
RUN dotnet build "Pi.GlobalMarketData.NonRealTimeDataHandler.csproj" -c Release -o /app/build
# RUN dotnet build "Pi.GlobalMarketData.NonRealTimeDataHandler.csproj" -c Debug -o /app/build

FROM build AS publish

# Todo: Should use `Release` mode
RUN dotnet publish "Pi.GlobalMarketData.NonRealTimeDataHandler.csproj" -c Release -o /app/publish /p:UseAppHost=false
# RUN dotnet publish "Pi.GlobalMarketData.NonRealTimeDataHandler.csproj" -c Debug -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 5003
ENTRYPOINT ["dotnet", "Pi.GlobalMarketData.NonRealTimeDataHandler.dll"]

# Command:
# docker build -t ge_non_realtime_data_handler -f Pi.GlobalMarketData.NonRealTimeDataHandler/Dockerfile .
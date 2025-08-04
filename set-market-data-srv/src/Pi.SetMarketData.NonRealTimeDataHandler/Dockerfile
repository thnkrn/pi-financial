# Use the official .NET 8 runtime image as the base image for the runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Use the official .NET 8 SDK image as the build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ["Pi.SetMarketData.NonRealTimeDataHandler/Pi.SetMarketData.NonRealTimeDataHandler.csproj", "Pi.SetMarketData.NonRealTimeDataHandler/"]
COPY ["Pi.SetMarketData.Application/Pi.SetMarketData.Application.csproj", "Pi.SetMarketData.Application/"]
COPY ["Pi.SetMarketData.Domain/Pi.SetMarketData.Domain.csproj", "Pi.SetMarketData.Domain/"]
COPY ["Pi.SetMarketData.Infrastructure/Pi.SetMarketData.Infrastructure.csproj", "Pi.SetMarketData.Infrastructure/"]
COPY ["nuget.config", ""]
RUN dotnet restore "Pi.SetMarketData.NonRealTimeDataHandler/Pi.SetMarketData.NonRealTimeDataHandler.csproj"
COPY . .
WORKDIR "/src/Pi.SetMarketData.NonRealTimeDataHandler"

# Todo: Should use `Release` mode
# RUN dotnet build "Pi.SetMarketData.NonRealTimeDataHandler.csproj" -c Release -o /app/build
RUN dotnet build "Pi.SetMarketData.NonRealTimeDataHandler.csproj" -c Debug -o /app/build

FROM build AS publish

# Todo: Should use `Release` mode
# RUN dotnet publish "Pi.SetMarketData.NonRealTimeDataHandler.csproj" -c Release -o /app/publish /p:UseAppHost=false
RUN dotnet publish "Pi.SetMarketData.NonRealTimeDataHandler.csproj" -c Debug -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 5003
ENTRYPOINT ["dotnet", "Pi.SetMarketData.NonRealTimeDataHandler.dll"]

# Command:
# docker build -t set_non_realtime_data_handler -f Pi.SetMarketData.NonRealTimeDataHandler/Dockerfile .
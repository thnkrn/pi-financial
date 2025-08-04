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
COPY ["Pi.SetMarketData.IndicatorWorker/Pi.SetMarketData.IndicatorWorker.csproj", "Pi.SetMarketData.IndicatorWorker/"]
COPY ["Pi.SetMarketData.Application/Pi.SetMarketData.Application.csproj", "Pi.SetMarketData.Application/"]
COPY ["Pi.SetMarketData.Domain/Pi.SetMarketData.Domain.csproj", "Pi.SetMarketData.Domain/"]
COPY ["Pi.SetMarketData.Infrastructure/Pi.SetMarketData.Infrastructure.csproj", "Pi.SetMarketData.Infrastructure/"]
COPY ["nuget.config", ""]
RUN dotnet restore "Pi.SetMarketData.IndicatorWorker/Pi.SetMarketData.IndicatorWorker.csproj"
COPY . .
WORKDIR "/src/Pi.SetMarketData.IndicatorWorker"

# Use `Release` mode
RUN dotnet build "Pi.SetMarketData.IndicatorWorker.csproj" -c Release -o /app/build
# RUN dotnet build "Pi.SetMarketData.IndicatorWorker.csproj" -c Debug -o /app/build

FROM build AS publish

# Use `Release` mode
RUN dotnet publish "Pi.SetMarketData.IndicatorWorker.csproj" -c Release -o /app/publish /p:UseAppHost=false
# RUN dotnet publish "Pi.SetMarketData.IndicatorWorker.csproj" -c Debug -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 5003
ENTRYPOINT ["dotnet", "Pi.SetMarketData.IndicatorWorker.dll"]

# Command:
# docker build -t set_indicator_worker -f Pi.SetMarketData.IndicatorWorker/Dockerfile .
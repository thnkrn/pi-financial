# Use the official .NET 8 runtime image as the base image for the runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Use the official .NET 8 SDK image as the build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Download the latest version of the tracer but don't install yet
#RUN TRACER_VERSION=$(curl -s \https://api.github.com/repos/DataDog/dd-trace-dotnet/releases/latest | grep tag_name | cut -d '"' -f 4 | cut -c2-) \
#    && curl -Lo /tmp/datadog-dotnet-apm.deb https://github.com/DataDog/dd-trace-dotnet/releases/download/v${TRACER_VERSION}/datadog-dotnet-apm_${TRACER_VERSION}_amd64.deb

WORKDIR /src
COPY ["Pi.SetMarketData.PreCacher/Pi.SetMarketData.PreCacher.csproj", "Pi.SetMarketData.PreCacher/"]
COPY ["Pi.SetMarketData.Application/Pi.SetMarketData.Application.csproj", "Pi.SetMarketData.Application/"]
COPY ["Pi.SetMarketData.Domain/Pi.SetMarketData.Domain.csproj", "Pi.SetMarketData.Domain/"]
COPY ["Pi.SetMarketData.Infrastructure/Pi.SetMarketData.Infrastructure.csproj", "Pi.SetMarketData.Infrastructure/"]
COPY ["nuget.config", ""]
RUN dotnet restore "Pi.SetMarketData.PreCacher/Pi.SetMarketData.PreCacher.csproj"
COPY . .
WORKDIR "/src/Pi.SetMarketData.PreCacher"

# Todo: Should use `Release` mode
# RUN dotnet build "Pi.SetMarketData.PreCacher.csproj" -c Release -o /app/build
RUN dotnet build "Pi.SetMarketData.PreCacher.csproj" -c Debug -o /app/build

FROM build AS publish

# Todo: Should use `Release` mode
# RUN dotnet publish "Pi.SetMarketData.PreCacher.csproj" -c Release -o /app/publish /p:UseAppHost=false
RUN dotnet publish "Pi.SetMarketData.PreCacher.csproj" -c Debug -o /app/publish /p:UseAppHost=false

FROM base AS final

# Copy the tracer from build target
#COPY --from=build /tmp/datadog-dotnet-apm.deb /tmp/datadog-dotnet-apm.deb

# Enable the tracer
#ARG DD_ENABLED=true

# Install the tracer
#RUN mkdir -p /opt/datadog \
#    && mkdir -p /var/log/datadog \
#    && dpkg -i /tmp/datadog-dotnet-apm.deb \
#    && rm /tmp/datadog-dotnet-apm.deb

#ENV CORECLR_ENABLE_PROFILING=${DD_ENABLED}
#ENV CORECLR_PROFILER={846F5F1C-F9AE-4B07-969E-05C26BC060D8}
#ENV CORECLR_PROFILER_PATH=/opt/datadog/Datadog.Trace.ClrProfiler.Native.so
#ENV DD_DOTNET_TRACER_HOME=/opt/datadog

WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Pi.SetMarketData.PreCacher.dll"]

# Command:
# docker build -t set_data_pre_cacher -f Pi.SetMarketData.PreCacher/Dockerfile .

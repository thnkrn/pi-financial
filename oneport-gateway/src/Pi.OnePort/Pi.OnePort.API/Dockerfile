#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 4000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Download the latest version of the tracer but don't install yet
RUN TRACER_VERSION=$(curl -s \https://api.github.com/repos/DataDog/dd-trace-dotnet/releases/latest | grep tag_name | cut -d '"' -f 4 | cut -c2-) \
    && curl -Lo /tmp/datadog-dotnet-apm.deb https://github.com/DataDog/dd-trace-dotnet/releases/download/v${TRACER_VERSION}/datadog-dotnet-apm_${TRACER_VERSION}_amd64.deb 

WORKDIR /src
COPY ["Pi.OnePort.API/Pi.OnePort.API.csproj", "Pi.OnePort.API/"]
COPY ["Pi.OnePort.Db2/Pi.OnePort.Db2.csproj", "Pi.OnePort.Db2/"]
COPY ["nuget.config", ""]
RUN dotnet restore "Pi.OnePort.API/Pi.OnePort.API.csproj"
COPY . .
WORKDIR "/src/Pi.OnePort.API"
RUN dotnet build "Pi.OnePort.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Pi.OnePort.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final

# Copy the tracer from build target
COPY --from=build /tmp/datadog-dotnet-apm.deb /tmp/datadog-dotnet-apm.deb
# Install the tracer
RUN mkdir -p /opt/datadog \
    && mkdir -p /var/log/datadog \
    && dpkg -i /tmp/datadog-dotnet-apm.deb \
    && rm /tmp/datadog-dotnet-apm.deb
# Enable the tracer
ARG DD_ENABLED
ENV CORECLR_ENABLE_PROFILING=${DD_ENABLED}
ENV CORECLR_PROFILER={846F5F1C-F9AE-4B07-969E-05C26BC060D8}
ENV CORECLR_PROFILER_PATH=/opt/datadog/Datadog.Trace.ClrProfiler.Native.so
ENV DD_DOTNET_TRACER_HOME=/opt/datadog

WORKDIR /app
RUN set -e; \
    apt-get update; \
    apt-get install -y libxml2-dev;

COPY --from=publish /app/publish .

ENV LD_LIBRARY_PATH=/app/clidriver/lib

ENTRYPOINT ["dotnet", "Pi.OnePort.API.dll"]

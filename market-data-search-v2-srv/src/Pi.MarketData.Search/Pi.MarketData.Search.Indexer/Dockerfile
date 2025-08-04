# See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# auto inserted by buildx
ARG TARGETARCH

# Download the latest version of the tracer but don't install yet
RUN apt update && apt install -y curl wget
# Add verification and explicit error handling for tracer download
RUN set -e \
    && TRACER_VERSION=$(curl -s https://api.github.com/repos/DataDog/dd-trace-dotnet/releases/latest | grep tag_name | cut -d '"' -f 4 | cut -c2-) \
    && echo "Downloading tracer version: ${TRACER_VERSION} for architecture: ${TARGETARCH}" \
    && DOWNLOAD_URL="https://github.com/DataDog/dd-trace-dotnet/releases/download/v${TRACER_VERSION}/datadog-dotnet-apm_${TRACER_VERSION}_${TARGETARCH}.deb" \
    && echo "Download URL: ${DOWNLOAD_URL}" \
    && wget --verbose -O /tmp/datadog-dotnet-apm.deb "${DOWNLOAD_URL}" \
    && ls -l /tmp/datadog-dotnet-apm.deb \
    && dpkg --info /tmp/datadog-dotnet-apm.deb || true

WORKDIR /src
COPY ["Pi.MarketData.Search.Indexer/Pi.MarketData.Search.Indexer.csproj", "Pi.MarketData.Search.Indexer/"]
COPY ["Pi.MarketData.Search.Application/Pi.MarketData.Search.Application.csproj", "Pi.MarketData.Search.Application/"]
COPY ["Pi.MarketData.Search.Domain/Pi.MarketData.Search.Domain.csproj", "Pi.MarketData.Search.Domain/"]
COPY ["nuget.config", ""]

# Skip Husky install on CI/CD
ENV HUSKY=0

RUN dotnet restore "Pi.MarketData.Search.Indexer/Pi.MarketData.Search.Indexer.csproj" -r linux-${TARGETARCH}
COPY . .
WORKDIR "/src/Pi.MarketData.Search.Indexer"
RUN dotnet build "Pi.MarketData.Search.Indexer.csproj" -c Release -o /app/build -r linux-${TARGETARCH}

FROM build AS publish
RUN dotnet publish "Pi.MarketData.Search.Indexer.csproj" -c Release -o /app/publish /p:UseAppHost=false -r linux-${TARGETARCH}

FROM base AS final
# Copy the tracer from build target
COPY --from=build /tmp/datadog-dotnet-apm.deb /tmp/datadog-dotnet-apm.deb

# Install the tracer with verification
RUN set -e \
    && mkdir -p /opt/datadog \
    && mkdir -p /var/log/datadog \
    && ls -l /tmp/datadog-dotnet-apm.deb \
    && dpkg --info /tmp/datadog-dotnet-apm.deb || true \
    && dpkg -i /tmp/datadog-dotnet-apm.deb \
    && rm /tmp/datadog-dotnet-apm.deb

# Enable the tracer
ARG DD_ENABLED
ENV CORECLR_ENABLE_PROFILING=${DD_ENABLED}
ENV CORECLR_PROFILER={846F5F1C-F9AE-4B07-969E-05C26BC060D8}
ENV CORECLR_PROFILER_PATH=/opt/datadog/Datadog.Trace.ClrProfiler.Native.so
ENV DD_DOTNET_TRACER_HOME=/opt/datadog

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pi.MarketData.Search.Indexer.dll"]
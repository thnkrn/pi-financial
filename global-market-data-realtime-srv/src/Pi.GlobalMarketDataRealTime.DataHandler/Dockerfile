# Use the official .NET 8 runtime image as the base image for the runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

RUN apt-get update && apt-get install -y stunnel4 tzdata procps dos2unix

# Install tzdata and set the time zone
# ENV TZ=Asia/Bangkok
ENV TZ=UTC
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

# Use the official .NET 8 SDK image as the build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Download the latest version of the tracer but don't install yet
RUN TRACER_VERSION=$(curl -s \https://api.github.com/repos/DataDog/dd-trace-dotnet/releases/latest | grep tag_name | cut -d '"' -f 4 | cut -c2-) \
    && curl -Lo /tmp/datadog-dotnet-apm.deb https://github.com/DataDog/dd-trace-dotnet/releases/download/v${TRACER_VERSION}/datadog-dotnet-apm_${TRACER_VERSION}_amd64.deb

WORKDIR /src
COPY ["Pi.GlobalMarketDataRealTime.DataHandler/Pi.GlobalMarketDataRealTime.DataHandler.csproj", "Pi.GlobalMarketDataRealTime.DataHandler/"]
COPY ["Pi.GlobalMarketDataRealTime.Application/Pi.GlobalMarketDataRealTime.Application.csproj", "Pi.GlobalMarketDataRealTime.Application/"]
COPY ["Pi.GlobalMarketDataRealTime.Domain/Pi.GlobalMarketDataRealTime.Domain.csproj", "Pi.GlobalMarketDataRealTime.Domain/"]
COPY ["Pi.GlobalMarketDataRealTime.Infrastructure/Pi.GlobalMarketDataRealTime.Infrastructure.csproj", "Pi.GlobalMarketDataRealTime.Infrastructure/"]
COPY ["nuget.config", ""]
RUN dotnet restore "Pi.GlobalMarketDataRealTime.DataHandler/Pi.GlobalMarketDataRealTime.DataHandler.csproj"
COPY . .
WORKDIR "/src/Pi.GlobalMarketDataRealTime.DataHandler"

RUN dotnet build "Pi.GlobalMarketDataRealTime.DataHandler.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Pi.GlobalMarketDataRealTime.DataHandler.csproj" -c Release -o /app/publish /p:UseAppHost=false

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
COPY --from=publish /app/publish .

RUN mkdir -p /etc/stunnel/certs /var/log/stunnel && \
    touch /var/log/stunnel/stunnel.log && \
    chmod -R 755 /etc/stunnel/certs && \
    chmod -R 755 /var/log/stunnel

# Copy stunnel configuration and start script
COPY Pi.GlobalMarketDataRealTime.DataHandler/stunnel.conf /etc/stunnel/
COPY Pi.GlobalMarketDataRealTime.DataHandler/Bridges_CA.crt /etc/stunnel/certs/
COPY Pi.GlobalMarketDataRealTime.DataHandler/start.sh /app/
RUN dos2unix /app/start.sh && chmod u+x /app/start.sh

ENTRYPOINT ["/app/start.sh"]
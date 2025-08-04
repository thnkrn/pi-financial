#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM ubuntu:22.04 AS cmd

FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy-chiseled-extra AS base
WORKDIR /app

FROM alpine/curl:latest AS datadog-agent
RUN TRACER_VERSION=$(curl -s \https://api.github.com/repos/DataDog/dd-trace-dotnet/releases/latest | grep tag_name | cut -d '"' -f 4 | cut -c2-) \
    && curl -Lo /tmp/datadog-dotnet-apm.tar.gz https://github.com/DataDog/dd-trace-dotnet/releases/download/v${TRACER_VERSION}/datadog-dotnet-apm-${TRACER_VERSION}.tar.gz
RUN mkdir /extracted-tracer
RUN tar -C /extracted-tracer -xzf /tmp/datadog-dotnet-apm.tar.gz
# Create an empty folder we can copy in the next stage to create the logs folder
RUN mkdir -p /empty

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Download the latest version of the tracer but don't install yet
# RUN TRACER_VERSION=$(curl -s \https://api.github.com/repos/DataDog/dd-trace-dotnet/releases/latest | grep tag_name | cut -d '"' -f 4 | cut -c2-) \
#     && curl -Lo /tmp/datadog-dotnet-apm.deb https://github.com/DataDog/dd-trace-dotnet/releases/download/v${TRACER_VERSION}/datadog-dotnet-apm_${TRACER_VERSION}_amd64.deb

WORKDIR /src
COPY ["Pi.TfexService.Worker/Pi.TfexService.Worker.csproj", "Pi.TfexService.Worker/"]
COPY ["Pi.TfexService.Application/Pi.TfexService.Application.csproj", "Pi.TfexService.Application/"]
COPY ["Pi.TfexService.Domain/Pi.TfexService.Domain.csproj", "Pi.TfexService.Domain/"]
COPY ["Pi.TfexService.Infrastructure/Pi.TfexService.Infrastructure.csproj", "Pi.TfexService.Infrastructure/"]
COPY ["Pi.TfexService.Migrations/Pi.TfexService.Migrations.csproj", "Pi.TfexService.Migrations/"]
COPY ["nuget.config", ""]
RUN dotnet restore "Pi.TfexService.Worker/Pi.TfexService.Worker.csproj"
COPY . .
WORKDIR "/src/Pi.TfexService.Worker"
RUN dotnet build "Pi.TfexService.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Pi.TfexService.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final

# Copy the tracer from build target
COPY --from=datadog-agent /extracted-tracer/ /opt/datadog/
COPY --from=datadog-agent /empty/ /var/log/datadog
COPY --from=cmd /bin/sleep /bin/sleep
# COPY --from=build /tmp/datadog-dotnet-apm.deb /tmp/datadog-dotnet-apm.deb
# # Install the tracer
# RUN mkdir -p /opt/datadog \
#     && mkdir -p /var/log/datadog \
#     && dpkg -i /tmp/datadog-dotnet-apm.deb \
#     && rm /tmp/datadog-dotnet-apm.deb
# Enable the tracer
ARG DD_ENABLED
ENV CORECLR_ENABLE_PROFILING=${DD_ENABLED}
ENV CORECLR_PROFILER={846F5F1C-F9AE-4B07-969E-05C26BC060D8}
ENV CORECLR_PROFILER_PATH=/opt/datadog/Datadog.Trace.ClrProfiler.Native.so
ENV DD_DOTNET_TRACER_HOME=/opt/datadog

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pi.TfexService.Worker.dll"]

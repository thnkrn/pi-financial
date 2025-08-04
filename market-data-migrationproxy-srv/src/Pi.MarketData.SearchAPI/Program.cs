using System.Globalization;
using System.Reflection;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Pi.Common.Helpers;
using Pi.Common.Startup.OpenTelemetry;
using Pi.MarketData.Infrastructure.Helpers;
using Pi.MarketData.Infrastructure.Services.Logging.Extensions;
using Serilog;
using Serilog.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using OpenSearch.Client;
using OpenSearch.Net;
using Pi.MarketData.SearchAPI.Startup;
using ConfigurationLogger = Pi.MarketData.Infrastructure.Services.Logging.ConfigurationLogger;

namespace Pi.MarketData.SearchAPI;

public static class Program
{
    public static void Main(string[] args)
    {
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");

        // Configure initial logger for startup
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        try
        {
            var configuration = ConfigurationHelper.GetConfiguration();
            var assembly = Assembly.GetExecutingAssembly();

            Log.Debug("Starting Application");
            ConfigurationLogger.LogConfigurations();

            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddConfiguration(configuration);

            // Configure Serilog
            builder.Host.UseSerilog((context, _, loggerConfiguration) =>
            {
                var options = new LoggingOptions
                {
                    ServiceType = "API",
                    Component = "MarketDataSearchAPI",
                    AdditionalOverrides = new Dictionary<string, LogEventLevel>
                    {
                        ["Microsoft.EntityFrameworkCore"] = LogEventLevel.Warning,
                        ["MassTransit"] = LogEventLevel.Warning
                    }
                };

                loggerConfiguration.ConfigureDefaultLogging(assembly, options);

                LoggingExtensions.ConfigureSinks(
                    loggerConfiguration,
                    context.Configuration,
                    context.HostingEnvironment);
            });

            ConfigurationLogger.LogConfigurations();

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddMemoryCache();
            builder.Services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.CustomSchemaIds(SwashbuckleHelpers.NormalizeModelName);
                options.MapType<decimal>(
                    () => new OpenApiSchema { Type = "number", Format = "decimal" }
                );

                // Make XML documentation optional
                var filePath = Path.Combine(AppContext.BaseDirectory, $"{assembly.GetName().Name}.xml");
                if (File.Exists(filePath))
                {
                    options.IncludeXmlComments(filePath);
                }

                options.EnableAnnotations();
            });
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHeaderPropagation(options => options.Headers.Add("correlation-id"));

            builder.Services.AddDbContexts(builder.Configuration);
            builder.Services.AddServices(builder.Configuration);
            builder.Services.AddHttpClients(builder.Configuration);

            builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy());

            // builder.Services.AddPiApiOpenTelemetry(
            //     builder.Configuration,
            //     builder.Environment.ApplicationName,
            //     assembly.ImageRuntimeVersion,
            //     [InstrumentationOptions.MeterName],
            //     [DiagnosticHeaders.DefaultListenerName],
            //     false
            // );
            builder.Services.AddProblemDetails();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    corsBuilder => corsBuilder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            });
            builder.Services.AddDbContexts(builder.Configuration);
            builder.Services.AddServices(builder.Configuration);

            var app = builder.Build();
            app.UseExceptionHandler();
            app.UseCors("AllowAll");

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapControllers();
            // Configure health checks endpoint
            app.MapHealthChecks("/", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            // Configure health checks endpoints
            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = _ => false,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.MapHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = healthCheck => healthCheck.Tags.Contains("ready"),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
            throw new InvalidOperationException(ex.Message, ex);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}

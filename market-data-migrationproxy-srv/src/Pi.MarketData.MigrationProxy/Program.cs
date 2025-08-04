using System.Globalization;
using System.Reflection;
using HealthChecks.UI.Client;
using MassTransit.Logging;
using MassTransit.Monitoring;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Pi.Common.Helpers;
using Pi.Common.Startup.OpenTelemetry;
using Pi.MarketData.Infrastructure.Helpers;
using Pi.MarketData.Infrastructure.Services.Logging;
using Pi.MarketData.Infrastructure.Services.Logging.Extensions;
using Pi.MarketData.MigrationProxy.API.Startup;
using Serilog;
using Serilog.Events;

namespace Pi.MarketData.MigrationProxy.API;

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

            Log.Information("Starting Application");
            ConfigurationLogger.LogConfigurations();

            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddConfiguration(configuration);


            // Configure Serilog using LoggingExtensions
            builder.Host.UseSerilog((context, _, loggerConfiguration) =>
            {
                var options = new LoggingOptions
                {
                    ServiceType = "API",
                    Component = "MarketDataMigrationProxy",
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

            builder.Services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.CustomSchemaIds(SwashbuckleHelpers.NormalizeModelName);
                options.MapType<decimal>(
                    () => new OpenApiSchema { Type = "number", Format = "decimal" }
                );
                var filePath = Path.Combine(
                    AppContext.BaseDirectory,
                    $"{assembly.GetName().Name}.xml"
                );
                options.IncludeXmlComments(filePath);
                options.EnableAnnotations();

                // Add route prefix configuration
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "MarketData Migration Proxy API", Version = "v1" });
            });

            // Add remaining services
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHeaderPropagation(options => options.Headers.Add("correlation-id"));
            builder.Services.AddDbContexts(builder.Configuration);
            builder.Services.SetupMassTransit(builder.Configuration, builder.Environment);
            builder.Services.AddHttpClients(builder.Configuration);
            builder.Services.AddServices(builder.Configuration);
            builder.Services.AddGrowthBookService(builder.Configuration);
            builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy());

            builder.Services.AddPiApiOpenTelemetry(
                builder.Configuration,
                builder.Environment.ApplicationName,
                assembly.ImageRuntimeVersion,
                [InstrumentationOptions.MeterName],
                [DiagnosticHeaders.DefaultListenerName],
                false
            );
            builder.Services.AddProblemDetails();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    corsBuilder => corsBuilder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            });
            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    ["application/json"]);
            });
            // Add services to the container.
            builder.Services.AddControllers(options =>
            {
                options.CacheProfiles.Add("Default5Mins", new CacheProfile
                {
                    Duration = 300,
                    Location = ResponseCacheLocation.Any,
                    VaryByHeader = "User-Agent"
                });
            });

            // Uncomment the memory cache line
            builder.Services.AddMemoryCache();

            var app = builder.Build();
            app.UseExceptionHandler();

            // Configure CORS - must be before routing
            app.UseCors("AllowAll");

            // Middleware to redirect "sub-path/swagger/v1/swagger.json" to "v1/swagger.json"
            app.Use(async (context, next) =>
            {
                var originalPath = context.Request.Path.Value;
                Log.Information("Request to {OriginalPath}", originalPath);
                if (context.Request.Path.StartsWithSegments("/swagger"))
                {
                    context.Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
                    context.Response.Headers.Append("Pragma", "no-cache");
                    context.Response.Headers.Append("Expires", "0");
                }

                await next();
            });

            // Configure the HTTP request pipeline.
            // Configure Swagger with the correct route prefix
            app.UseSwagger(c =>
            {
                var path = app.Environment.IsDevelopment()
                    ? "/swagger/{documentName}/swagger.json"
                    : "/{documentName}/swagger.json";

                Log.Information("Path UseSwagger : {Path}", path);
                c.RouteTemplate = path;
            });
            app.UseSwaggerUI(c =>
            {
                var basePath = app.Environment.IsDevelopment()
                    ? "/swagger/v1/swagger.json"
                    : "/v1/swagger.json";

                c.SwaggerEndpoint(basePath, "Market Data Migration Proxy API V1");

                Log.Information("RoutePrefix : {RoutePrefix}", c.RoutePrefix);
                Log.Information("Path : {BasePath}", basePath);
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "")),
                RequestPath = "/swagger",
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
                }
            });

            Log.Information(Directory.GetCurrentDirectory());

            // Important: UseRouting should come before UseEndpoints but after Swagger
            app.UseRouting();
            app.UseAuthorization();
            app.UseResponseCompression();

            var webSocketOptions = new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromMinutes(3),
                AllowedOrigins = { "*" }
            };

            app.UseWebSockets(webSocketOptions);

            // Health checks endpoints
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

            // Map controllers last
            app.MapControllers();
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
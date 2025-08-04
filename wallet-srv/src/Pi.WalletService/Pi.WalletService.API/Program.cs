using System.Reflection;
using MassTransit.Logging;
using MassTransit.Monitoring;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Pi.Common.Helpers;
using Pi.Common.Serilog;
using Pi.Common.Startup.OpenTelemetry;
using Pi.WalletService.API.Startup;
using Pi.WalletService.Infrastructure.Services;
using Serilog;
using Serilog.Debugging;

namespace Pi.WalletService.API;

public static class Program
{
    public static void Main(string[] args)
    {
        var configuration = GetConfiguration();

        Log.Logger = PiSerilogConfiguration.CreateBootstrapLogger();
        SelfLog.Enable(Console.Error);

        try
        {
            var builder = WebApplication.CreateBuilder(args);
            var assembly = Assembly.GetExecutingAssembly();

            builder.Configuration.AddConfiguration(configuration);

            // Add services to the container.
            builder.Services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });
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
                options.MapType<decimal>(() => new OpenApiSchema { Type = "number", Format = "decimal" });
                var filePath = Path.Combine(AppContext.BaseDirectory, $"{assembly.GetName().Name}.xml");
                options.IncludeXmlComments(filePath);
            });
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHeaderPropagation(options => options.Headers.Add("correlation-id"));
            builder.Services.AddDbContexts(builder.Configuration, builder.Environment);
            builder.Services.SetupMassTransit(builder.Configuration, builder.Environment);
            builder.Services.AddHttpClients(builder.Configuration);
            builder.Services.AddServices(builder.Configuration);
            builder.Services.AddHealthChecks();
            builder.Services.AddProblemDetails();
            builder.Services.AddSwaggerGenNewtonsoftSupport();
            builder.Services.AddPiApiOpenTelemetry(
                builder.Configuration,
                builder.Environment.ApplicationName,
                assembly.ImageRuntimeVersion,
                new[] { MetricService.MeterName, InstrumentationOptions.MeterName },
                new[] { DiagnosticHeaders.DefaultListenerName },
                false);
            builder.Host.UsePiApiSerilog();

            var app = builder.Build();
            app.UseSerilogRequestLogging();
            app.UseExceptionHandler();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Prevent access to /debug on Production
            if (app.Environment.IsProduction())
            {
                app.Use(async (context, next) =>
                {
                    if (context.Request.Path.StartsWithSegments("/debug"))
                    {
                        context.Response.StatusCode = 404;
                        return;
                    }
                    await next.Invoke();
                });
            }

            app.UseHeaderPropagation();
            app.UseAuthorization();
            app.MapControllers();
            app.MapHealthChecks("/");
            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static IConfiguration GetConfiguration()
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{env}.json", true)
            .AddEnvironmentVariables();

        var config = builder.Build();
        if (config.GetValue<bool>("RemoteConfig:Enable"))
        {
            var prefix = config.GetValue<string>("RemoteConfig:Prefix");
            var lifetime = config.GetValue<double>("RemoteConfig:LifetimeMS");
            builder.AddSystemsManager($"/{env.ToLowerInvariant()}/{prefix}", TimeSpan.FromMilliseconds(lifetime));

            return builder.Build();
        }

        return config;
    }
}
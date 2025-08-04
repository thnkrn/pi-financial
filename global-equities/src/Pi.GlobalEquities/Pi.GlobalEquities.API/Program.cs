using System.Reflection;
using MassTransit.Logging;
using MassTransit.Monitoring;
using Microsoft.OpenApi.Models;
using Pi.Common.Serilog;
using Pi.Common.Startup.OpenTelemetry;
using Serilog;
using Serilog.Debugging;

namespace Pi.GlobalEquities.API;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            Log.Logger = PiSerilogConfiguration.CreateBootstrapLogger();
            SelfLog.Enable(Console.Error);

            var config = GetConfiguration();
            var builder = WebApplication.CreateBuilder(args);
            var assembly = Assembly.GetExecutingAssembly();

            builder.Host.UsePiApiSerilog();

            builder.Logging.ClearProviders();

            builder.Configuration.AddConfiguration(config);

            var services = builder.Services;
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.MapType<decimal>(() => new OpenApiSchema { Type = "number", Format = "decimal" });
                string filePath = Path.Combine(AppContext.BaseDirectory, $"{assembly.GetName().Name}.xml");
                options.IncludeXmlComments(filePath);
            });
            services.AddHttpContextAccessor();
            services.AddHeaderPropagation(options => options.Headers.Add("correlation-id"));
            services.AddHealthChecks();
            services.AddPiApiOpenTelemetry(
                builder.Configuration,
                builder.Environment.ApplicationName,
                assembly.ImageRuntimeVersion,
                new[] { InstrumentationOptions.MeterName },
                new[] { DiagnosticHeaders.DefaultListenerName },
                false);
            services.AddProblemDetails();

            var env = builder.Environment;

            var startup = new Startup(config, env);
            startup.ConfigureServices(services, builder.Configuration);

            var app = builder.Build();
            startup.ConfigureApp(app);

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
        IConfigurationBuilder builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{env}.json", true)
            .AddEnvironmentVariables();

        var config = builder.Build();
        if (config.GetValue<bool>("RemoteConfig:Enable"))
        {
            string prefix = config.GetValue<string>("RemoteConfig:Prefix");
            double lifetime = config.GetValue<double>("RemoteConfig:LifetimeMS");
            builder.AddSystemsManager($"/{env.ToLowerInvariant()}/{prefix}", TimeSpan.FromMilliseconds(lifetime));

            return builder.Build();
        }

        return config;
    }
}

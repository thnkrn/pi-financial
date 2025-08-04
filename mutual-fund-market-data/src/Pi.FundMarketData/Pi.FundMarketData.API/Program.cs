using System.Reflection;
using System.Text.Json.Serialization;
using MassTransit.Logging;
using MassTransit.Monitoring;
using Microsoft.OpenApi.Models;
using Pi.Common.Helpers;
using Pi.Common.Serilog;
using Pi.Common.Startup.OpenTelemetry;
using Pi.FundMarketData.API.Converters;
using Pi.FundMarketData.API.Startup;
using Serilog;
using Serilog.Debugging;

public class Program
{
    public static void Main(string[] args)
    {
        IConfiguration configuration = GetConfiguration();

        Log.Logger = PiSerilogConfiguration.CreateBootstrapLogger();
        SelfLog.Enable(Console.Error);

        try
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            Assembly assembly = Assembly.GetExecutingAssembly();

            // Add log
            builder.Logging.ClearProviders();

            builder.Configuration.AddConfiguration(configuration);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddMemoryCache();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.Converters.Add(new DoubleRoundingConverter());
                    options.JsonSerializerOptions.Converters.Add(new DecimalRoundingConverter());
                });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.CustomSchemaIds(SwashbuckleHelpers.NormalizeModelName);
                options.MapType<decimal>(() => new OpenApiSchema { Type = "number", Format = "decimal" });
                string filePath = Path.Combine(AppContext.BaseDirectory, $"{assembly.GetName().Name}.xml");
                options.IncludeXmlComments(filePath);
            });
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHeaderPropagation(options => options.Headers.Add("correlation-id"));
            builder.Services.AddDbContexts(builder.Configuration);
            builder.Services.SetupMassTransit(builder.Configuration, builder.Environment);
            builder.Services.AddHttpClients(builder.Configuration);
            builder.Services.AddServices(builder.Configuration);
            builder.Services.AddHealthChecks();
            builder.Services.AddPiApiOpenTelemetry(
                builder.Configuration,
                builder.Environment.ApplicationName,
                assembly.ImageRuntimeVersion,
                new[] { InstrumentationOptions.MeterName },
                new[] { DiagnosticHeaders.DefaultListenerName },
                false);
            builder.Host.UsePiApiSerilog();
            builder.Services.AddProblemDetails();

            WebApplication app = builder.Build();

            app.UseExceptionHandler();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

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
        string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        IConfigurationBuilder builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{env}.json", true)
            .AddEnvironmentVariables();

        IConfigurationRoot config = builder.Build();
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

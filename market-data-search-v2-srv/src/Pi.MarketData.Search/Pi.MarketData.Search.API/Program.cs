using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Pi.Common.Helpers;
using Pi.Common.Serializations;
using Pi.Common.Serilog;
using Pi.Common.Startup.OpenTelemetry;
using Pi.MarketData.Search.API.Converters;
using Pi.MarketData.Search.API.Startup;
using Serilog;
using Serilog.Debugging;

namespace Pi.MarketData.Search.API;

public class Program
{
    public static void Main(string[] args)
    {
        var configuration = GetConfiguration();

        Log.Logger = PiSerilogConfiguration.CreateBootstrapLogger();
        SelfLog.Enable(Console.Error);

        var cultureInfo = new CultureInfo("en-US")
        {
            DateTimeFormat = { Calendar = new GregorianCalendar() }
        };

        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

        Thread.CurrentThread.CurrentCulture = cultureInfo;
        Thread.CurrentThread.CurrentUICulture = cultureInfo;

        try
        {
            var builder = WebApplication.CreateBuilder(args);
            var assembly = Assembly.GetExecutingAssembly();

            // Add log
            builder.Logging.ClearProviders();

            builder.Configuration.AddConfiguration(configuration);

            builder.Services.AddHttpClient();

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddMemoryCache();
            builder.Services.AddDistributedMemoryCache();
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
                options.MapType<decimal>(() => new OpenApiSchema { Type = "string", Format = "double", Example = new OpenApiDouble(1112.1314) });
                options.MapType<double>(() => new OpenApiSchema { Type = "string", Format = "double", Example = new OpenApiDouble(1112.1314) });
                var filePath = Path.Combine(AppContext.BaseDirectory, $"{assembly.GetName().Name}.xml");
                options.IncludeXmlComments(filePath);
            });
            builder.Services
                .AddControllers(options => { options.AllowEmptyInputInBodyModelBinding = true; })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase,
                        false));
                    options.JsonSerializerOptions.Converters.Add(new StringDecimalJsonConverter() { Digit = 5 });
                    options.JsonSerializerOptions.Converters.Add(new JsonDateTimeKindConverter(DateTimeKind.Utc));
                });
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHeaderPropagation(options => options.Headers.Add("correlation-id"));
            builder.Services.AddCacheService(builder.Configuration);
            builder.Services.AddDbContexts(builder.Configuration);
            builder.Services.AddHttpClients(builder.Configuration);
            builder.Services.AddServices(builder.Configuration);
            builder.Services.AddHealthChecks();
            builder.Services.AddHttpLogging(o => { o.LoggingFields = HttpLoggingFields.All; });
            builder.Services.AddPiApiOpenTelemetry(
                builder.Configuration,
                builder.Environment.ApplicationName,
                assembly.ImageRuntimeVersion,
                [],
                [],
                false);
            builder.Host.UsePiApiSerilog();
            builder.Services.AddProblemDetails();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    corsBuilder => corsBuilder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            });

            var app = builder.Build();

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

            app.Lifetime.ApplicationStarted.Register(() =>
            {
                var urls = app.Urls;
                Console.WriteLine("Application started at:");
                foreach (var url in urls)
                {
                    Console.WriteLine($"  {url}");
                    if (!app.Environment.IsProduction())
                    {
                        Console.WriteLine($"  {url}/swagger");
                    }
                }
            });

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

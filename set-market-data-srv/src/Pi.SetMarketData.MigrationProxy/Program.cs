using System.Globalization;
using System.Reflection;
using MassTransit.Logging;
using MassTransit.Monitoring;
using Microsoft.OpenApi.Models;
using Pi.Common.Helpers;
using Pi.Common.Startup.OpenTelemetry;
using Pi.SetMarketData.MigrationProxy.Handlers;
using Pi.SetMarketData.MigrationProxy.Startup;
using Serilog;

namespace Pi.SetMarketData.MigrationProxy;

public static class Program
{
    public static void Main(string[] args)
    {
        var configuration = GetConfiguration();
        Serilog.Debugging.SelfLog.Enable(Console.Error);
        CultureInfo cultureInfo = new("en-US");
        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;

        try
        {
            var builder = WebApplication.CreateBuilder(args);
            var assembly = Assembly.GetExecutingAssembly();


            builder.Configuration.AddConfiguration(configuration);

            // Add services to the container.
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<HandleExceptionFilter>();
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
                options.MapType<decimal>(
                    () => new OpenApiSchema { Type = "number", Format = "decimal" }
                );
                var filePath = Path.Combine(
                    AppContext.BaseDirectory,
                    $"{assembly.GetName().Name}.xml"
                );
                options.IncludeXmlComments(filePath);
                options.EnableAnnotations();
            });
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHeaderPropagation(options => options.Headers.Add("correlation-id"));
            builder.Services.AddDbContexts(builder.Configuration);
            builder.Services.SetupMassTransit(builder.Configuration, builder.Environment);
            builder.Services.AddHttpClients(builder.Configuration);
            builder.Services.AddServices(builder.Configuration);
            builder.Services.AddGrowthBookService(builder.Configuration);
            builder.Services.AddHealthChecks();

            builder.Services.AddPiApiOpenTelemetry(
                builder.Configuration,
                builder.Environment.ApplicationName,
                assembly.ImageRuntimeVersion,
                new[] { InstrumentationOptions.MeterName },
                new[] { DiagnosticHeaders.DefaultListenerName },
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

            var webSocketOptions = new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromMinutes(2)
            };
            app.UseWebSockets(webSocketOptions);

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
            builder.AddSystemsManager(
                $"/{env.ToLowerInvariant()}/{prefix}",
                TimeSpan.FromMilliseconds(lifetime)
            );

            return builder.Build();
        }

        return config;
    }
}
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using MassTransit.Logging;
using MassTransit.Monitoring;
using Microsoft.OpenApi.Models;
using Pi.BackofficeService.API.Middlewares;
using Pi.BackofficeService.API.Startup;
using Pi.BackofficeService.Infrastructure.Services;
using Pi.Common.Helpers;
using Pi.Common.Serializations;
using Pi.Common.Serilog;
using Pi.Common.Startup.OpenTelemetry;
using Serilog;
using Serilog.Debugging;

namespace Pi.BackofficeService.API;

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

            // Add log
            builder.Logging.ClearProviders();
            builder.Configuration.AddConfiguration(configuration);

            // Add services to the container.
            builder.Services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, false));
                    options.JsonSerializerOptions.Converters.Add(new JsonDateTimeKindConverter(DateTimeKind.Utc));
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
                var filePath = Path.Combine(AppContext.BaseDirectory, $"{assembly.GetName().Name}.xml");
                options.IncludeXmlComments(filePath);
                options.CustomSchemaIds(SwashbuckleHelpers.NormalizeModelName);
                options.UseInlineDefinitionsForEnums();
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHeaderPropagation(options => options.Headers.Add("X-Correlation-ID"));
            builder.Services.AddDbContexts(builder.Configuration);
            builder.Services.SetupMassTransit(builder.Configuration, builder.Environment);
            builder.Services.AddHttpClients(builder.Configuration);
            builder.Services.AddServices(builder.Configuration, builder.Environment);
            builder.Services.SetupPermissions(builder.Configuration);
            builder.Services.AddHealthChecks();
            builder.Services.AddProblemDetails();
            builder.Services.AddResponseCaching();
            builder.Services.AddPiApiOpenTelemetry(
                builder.Configuration,
                builder.Environment.ApplicationName,
                assembly.ImageRuntimeVersion,
                new[] { MetricService.MeterName, InstrumentationOptions.MeterName },
                new[] { DiagnosticHeaders.DefaultListenerName },
                false);
            builder.Host.UsePiApiSerilog();

            var CorsPolicy = "Cors";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicy, policy =>
                {
                    var origins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>();
                    if (origins != null)
                    {
                        policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod();
                    }
                });
            });

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

            app.UseCors(CorsPolicy);
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseResponseCaching();
            app.MapControllers();
            app.UseMiddleware<ErrorResponseMiddleware>();
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

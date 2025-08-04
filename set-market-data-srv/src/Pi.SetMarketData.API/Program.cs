using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Models;
using Pi.Common.Helpers;
using Pi.SetMarketData.API.Infrastructure.ExceptionHandlers;
using Pi.SetMarketData.API.Startup;
using Pi.SetMarketData.Infrastructure.Converters;
using Pi.SetMarketData.Infrastructure.Helpers;
using Pi.SetMarketData.Infrastructure.Services.Logging;
using Pi.SetMarketData.Infrastructure.Services.Logging.Extensions;
using Serilog;
using Serilog.Events;
using LoggingExtensions = Pi.SetMarketData.Infrastructure.Services.Logging.Extensions.LoggingExtensions;

namespace Pi.SetMarketData.API;

public static class Program
{
    public static void Main(string[] args)
    {
        CultureInfo cultureInfo = new("en-US");
        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;

        // Configure initial logger for startup
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        var configuration = ConfigurationHelper.GetConfiguration();

        try
        {
            var builder = WebApplication.CreateBuilder(args);
            var assembly = Assembly.GetExecutingAssembly();

            // Log startup information early
            Log.Debug("Starting {ApplicationName} in {Environment}",
                builder.Environment.ApplicationName,
                builder.Environment.EnvironmentName);

            ConfigurationLogger.LogConfigurations();

            // Configure full Serilog using LoggingExtensions
            builder.Host.UseSerilog((context, _, loggerConfiguration) =>
            {
                var options = new LoggingOptions
                {
                    ServiceType = "API",
                    Component = "API",
                    AdditionalOverrides = new Dictionary<string, LogEventLevel>
                    {
                        ["Microsoft.EntityFrameworkCore"] = LogEventLevel.Warning
                    },
                    AdditionalProperties = new Dictionary<string, string>
                    {
                        ["Environment"] = context.HostingEnvironment.EnvironmentName
                    }
                };

                loggerConfiguration.ConfigureDefaultLogging(assembly, options);
                LoggingExtensions.ConfigureSinks(
                    loggerConfiguration,
                    context.Configuration,
                    context.HostingEnvironment);
            });

            // Add configuration
            builder.Configuration.AddConfiguration(configuration);

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
                var filePath = Path.Combine(
                    AppContext.BaseDirectory,
                    $"{assembly.GetName().Name}.xml"
                );
                options.IncludeXmlComments(filePath);
                options.EnableAnnotations();
            });

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHeaderPropagation(options => options.Headers.Add("correlation-id"));
            builder.Services.AddCacheService(builder.Configuration);
            builder.Services.AddDbContexts(builder.Configuration);
            builder.Services.SetupMassTransit(builder.Configuration, builder.Environment);
            builder.Services.AddHttpClients(builder.Configuration);
            builder.Services.AddServices(builder.Configuration);
            builder.Services.AddHealthChecks();
            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    ["application/json"]);
            });

            builder.Services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new ObjectIdConverter());
                });

            // Configure ProblemDetails and ExceptionHandler
            builder.Services.AddExceptionHandler<GlobalProblemDetailsHandler>();
            builder.Services.AddProblemDetails(options =>
                options.CustomizeProblemDetails = GlobalProblemDetailsHandler.CustomizeProblemDetails);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    corsPolicyBuilder => corsPolicyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod() //NOSONAR
                );
            });

            var app = builder.Build();
            Log.Debug("Application built successfully");

            app.UseExceptionHandler();
            app.UseCors("AllowAll");

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthorization();
            app.UseResponseCompression();
            app.MapControllers();
            app.MapHealthChecks("/");

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
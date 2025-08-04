using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using MassTransit.Logging;
using MassTransit.Monitoring;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Pi.Common.Helpers;
using Pi.Common.Serializations;
using Pi.Common.Serilog;
using Pi.Common.Startup.OpenTelemetry;
using Pi.Financial.FundService.API.Middlewares;
using Pi.Financial.FundService.API.Models;
using Pi.Financial.FundService.API.Startup;
using Serilog;
using Serilog.Configuration;
using Serilog.Debugging;

var configuration = GetConfiguration();
Log.Logger = PiSerilogConfiguration.CreateBootstrapLogger();
SelfLog.Enable(Console.Error);

try
{
    var builder = WebApplication.CreateBuilder(args);
    var assembly = Assembly.GetExecutingAssembly();

    // Add Log
    builder.Logging.ClearProviders();

    builder.Configuration.AddConfiguration(configuration);

    // Add services to the container.
    builder.Services
        .AddControllers(options => { options.AllowEmptyInputInBodyModelBinding = true; })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase,
                false));
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
        options.CustomSchemaIds(SwashbuckleHelpers.NormalizeModelName);
        var filePath = Path.Combine(AppContext.BaseDirectory, $"{assembly.GetName().Name}.xml");
        options.IncludeXmlComments(filePath);
        options.UseInlineDefinitionsForEnums();
    });
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddHeaderPropagation(options => options.Headers.Add("X-Correlation-ID"));
    builder.Services.AddDbContexts(builder.Configuration);
    builder.Services.SetupMassTransit(builder.Configuration, builder.Environment);
    builder.Services.AddHttpClients(builder.Configuration);
    builder.Services.AddServices(builder.Configuration);
    builder.Services.AddHealthChecks();
    builder.Services.AddProblemDetails();
    builder.Services.AddHttpLogging(o => { o.LoggingFields = HttpLoggingFields.All; });
    builder.Services.AddPiApiOpenTelemetry(
        builder.Configuration,
        builder.Environment.ApplicationName,
        assembly.ImageRuntimeVersion,
        new[] { InstrumentationOptions.MeterName },
        new[] { DiagnosticHeaders.DefaultListenerName },
        false
    );
    builder.Host.UsePiApiSerilog();

    var app = builder.Build();

    app.UseMiddleware<CustomLoggingContextMiddleware>();
    app.UseSerilogRequestLogging();
    app.UseExceptionHandler();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsProduction())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI();
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

IConfiguration GetConfiguration()
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

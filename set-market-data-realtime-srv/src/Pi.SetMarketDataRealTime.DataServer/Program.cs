using System.Reflection;
using MassTransit.Logging;
using MassTransit.Monitoring;
using Pi.Common.Serilog;
using Pi.Common.Startup.OpenTelemetry;
using Pi.SetMarketDataRealTime.DataServer;
using Pi.SetMarketDataRealTime.DataServer.Helpers;
using Pi.SetMarketDataRealTime.DataServer.Services.SoupBinTcp;
using Pi.SetMarketDataRealTime.DataServer.Startup;
using Serilog;
using Serilog.Debugging;

var configuration = ConfigurationHelper.GetConfiguration();

Log.Logger = PiSerilogConfiguration.CreateBootstrapLogger();
SelfLog.Enable(Console.Error);
try
{
    Log.Information("Starting host");
    var assembly = Assembly.GetExecutingAssembly();

    var hostBuilder = Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration(builder => builder.AddConfiguration(configuration))
        .ConfigureServices((hostContext, services) =>
        {
            services.AddMemoryCache();
            services.AddDbContexts(hostContext.Configuration);
            services.SetupMassTransit(hostContext.Configuration, hostContext.HostingEnvironment);
            services.AddServices(hostContext.Configuration);
            services.AddHttpClients(hostContext.Configuration);
            services.AddPiWorkerOpenTelemetry(
                hostContext.Configuration,
                hostContext.HostingEnvironment.ApplicationName,
                assembly.ImageRuntimeVersion,
                [InstrumentationOptions.MeterName],
                [DiagnosticHeaders.DefaultListenerName],
                false);
        })
        .UsePiWorkerSerilog();

    _ = hostBuilder.Build().RunAsync();

    Console.WriteLine("Starting server...");
    var server = new Server(new ServerListener());
    server.Start();

    var command = string.Empty;
    while (command != "x") command = Console.ReadLine();

    server.Shutdown();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}
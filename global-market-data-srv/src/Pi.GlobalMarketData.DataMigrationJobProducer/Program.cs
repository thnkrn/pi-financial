using System.Globalization;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pi.GlobalMarketData.DataMigrationJobProducer.Services;
using Pi.GlobalMarketData.DataMigrationJobProducer.Startup;
using Pi.GlobalMarketData.Infrastructure.Helpers;
using Pi.GlobalMarketData.Infrastructure.Services.Logging.Extensions;
using Serilog;
using Serilog.Events;

namespace Pi.GlobalMarketData.DataMigrationJobProducer;

public static class Program
{
    private static readonly HashSet<string> AllowVenue = ["ARCA", "BATS", "HKEX", "NASDAQ", "NYSE"];

    private static async Task Main(string[] args)
    {
        // Configure initial logger for startup
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        try
        {
            Log.Debug("Starting application initialization...");
            var assembly = Assembly.GetExecutingAssembly();

            if (args.Length < 4)
            {
                Log.Warning("Insufficient arguments provided");
                Console.WriteLine(
                    "Usage: dotnet run -- \"{from-date}\" \"{to-date}\" \"{venue}\" \"{symbol},{symbol},...,{symbol}\"");
                return;
            }

            if (!TryParseArgs(args, out var migrationDateFrom, out var migrationDateTo, out var venue,
                    out var stockSymbols)) return;

            Log.Debug(
                "Starting migration job producer with parameters: FromDate={FromDate}, ToDate={ToDate}, Venue={Venue}, Symbols={Symbols}",
                migrationDateFrom,
                migrationDateTo,
                venue,
                string.Join(",", stockSymbols));

            var configuration = ConfigurationHelper.GetConfiguration();
            var host = CreateHostBuilder(args, configuration, assembly).Build();

            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var migrationJobsProducer = services.GetRequiredService<MigrationJobsProducerService>();
                await migrationJobsProducer.ProduceMigrationJobsAsync(migrationDateFrom, migrationDateTo, venue,
                    stockSymbols);

                Log.Debug("Migration jobs produced successfully");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while producing migration jobs");
                throw new InvalidOperationException("An error occurred while producing migration jobs", ex);
            }
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Service terminated unexpectedly");
            throw new InvalidOperationException(ex.Message, ex);
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }

    public static bool TryParseArgs(string[] args, out DateTime migrationDateFrom, out DateTime migrationDateTo,
        out string venue, out string[] stockSymbols)
    {
        migrationDateFrom = default;
        migrationDateTo = default;
        venue = args[2];
        stockSymbols = [];

        if (!DateTime.TryParseExact(args[0], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal,
                out migrationDateFrom) ||
            !DateTime.TryParseExact(args[1], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal,
                out migrationDateTo))
        {
            Console.WriteLine("Invalid date format. Please use 'yyyy-MM-dd'.");
            return false;
        }

        if (!AllowVenue.Contains(venue))
        {
            Console.WriteLine($"Venue: {venue}, is not allow for SET Domain");
            return false;
        }

        stockSymbols = args[3].Split(',', StringSplitOptions.RemoveEmptyEntries);
        if (stockSymbols.Length == 0)
        {
            Console.WriteLine("No stock symbols provided.");
            return false;
        }

        return true;
    }

    public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration, Assembly assembly)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(builder => builder.AddConfiguration(configuration))
            .UseSerilog((context, _, loggerConfiguration) =>
            {
                var options = new LoggingOptions
                {
                    ServiceType = "Producer",
                    Component = "DataMigration",
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
            })
            .ConfigureServices((_, services) =>
            {
                services.AddLogging();
                services.AddServices(configuration);
                services.AddSingleton<MigrationJobsProducerService>();
            });
    }
}
using System.Globalization;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pi.SetMarketData.DataMigrationJobProducer.Services;
using Pi.SetMarketData.DataMigrationJobProducer.Startup;
using Pi.SetMarketData.Infrastructure.Helpers;
using Pi.SetMarketData.Infrastructure.Services.Logging.Extensions;
using Serilog;
using Serilog.Events;

namespace Pi.SetMarketData.DataMigrationJobProducer;

public static class Program
{
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

            if (args.Length < 5)
            {
                Log.Warning("Insufficient arguments provided");
                Console.WriteLine(
                    "Usage: dotnet run -- \"{migration-type}\" \"{from-date}\" \"{to-date}\" \"{venue}\" \"{symbol},{symbol},...,{symbol}\" [{timeframe]}");
                return;
            }

            if (!TryParseArgs(args, out var migrationType, out var migrationDateFrom, out var migrationDateTo, out var venue,
                    out var stockSymbols, out var timeframe)) return;

            Log.Debug(
                "Starting migration job producer with parameters: MigrationType={MigrationType}, FromDate={FromDate}, ToDate={ToDate}, Venue={Venue}, Symbols={Symbols}",
                migrationType,
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
                if (migrationType == "ohlc")
                {
                    await migrationJobsProducer.ProduceMigrationJobsAsync(migrationDateFrom, migrationDateTo, venue,
                    stockSymbols);
                }
                else if (migrationType == "indicator")
                {
                    await migrationJobsProducer.ProduceIndicatorsMigrationJobsAsync(migrationDateFrom, migrationDateTo, venue,
                    stockSymbols, timeframe);
                }


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

    public static bool TryParseArgs(string[] args, out string migrationType, out DateTime migrationDateFrom, out DateTime migrationDateTo,
        out string venue, out string[] stockSymbols, out string timeframe)
    {
        migrationType = args[0];
        migrationDateFrom = default;
        migrationDateTo = default;
        venue = args[3];
        stockSymbols = args[4].Split(',', StringSplitOptions.RemoveEmptyEntries);
        timeframe = "";

        if (migrationType != "ohlc" && migrationType != "indicator")
        {
            Log.Error("Invalid migration type provided: {MigrationType}", migrationType);
            Console.WriteLine("Invalid migration type. Please use 'ohlc' or 'indicators'.");
            return false;
        }

        if (migrationType == "indicator")
        {
            if (args.Length < 6)
            {
                Log.Error("Timeframe is required for indicator migration");
                Console.WriteLine("Usage: dotnet run -- \"{migration-type}\" \"{from-date}\" \"{to-date}\" \"{venue}\" \"{symbol},{symbol},...,{symbol}\" [{timeframe}]");
                return false;
            }

            timeframe = args[5];
            var allowTimeframe = new HashSet<string>(["1min", "2min", "5min", "15min", "30min", "1hour", "2hour", "4hour", "1day", "1week", "1month"]);
            if (!allowTimeframe.Contains(timeframe))
            {
                Log.Error("Invalid timeframe provided: {Timeframe}", timeframe);
                Console.WriteLine("Invalid timeframe. Please use '1min', '2min', '5min', '15min', '30min', '1hour', '2hour', '4hour', '1day', '1week', '1month'.");
                return false;
            }
        }

        if (!DateTime.TryParseExact(args[1], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal,
                out migrationDateFrom) ||
            !DateTime.TryParseExact(args[2], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal,
                out migrationDateTo))
        {
            Log.Error("Invalid date format provided: {FromDate}, {ToDate}", args[0], args[1]);
            Console.WriteLine("Invalid date format. Please use 'yyyy-MM-dd'.");
            return false;
        }

        // ReSharper disable CollectionNeverUpdated.Local
        var allowVenue = new HashSet<string>(["Equity", "Derivative", "ARCA", "BATS", "HKEX", "NASDAQ", "NYSE"]);
        if (!allowVenue.Contains(venue))
        {
            Log.Error("Invalid venue provided: {Venue}", venue);
            Console.WriteLine($"Venue: {venue}, is not allow for SET Domain");
            return false;
        }

        if (stockSymbols.Length == 0)
        {
            Log.Error("No stock symbols provided");
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
// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pi.SetMarketDataWSS.DataStreamer;
using Pi.SetMarketDataWSS.Domain.Models.Response;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

OptionToRun:
Console.WriteLine("Please enter 1, 2, or 3.");
Console.WriteLine("1: to run StreamDataToSignalRHub function.");
Console.WriteLine("2: to run StreamDataToDataSubscriber function.");
Console.WriteLine("3: to run away!");
Console.WriteLine("=================================");

var key = Console.ReadLine();
switch (key)
{
    case "1":
        await StreamDataToSignalRHub();
        break;
    case "2":
        await StreamDataToDataSubscriber();
        break;
    case "3":
        Environment.Exit(0);
        break;
    default:
        Console.WriteLine("Invalid selection. Please enter 1, 2, or 3.");
        goto OptionToRun;
}

return;

async Task StreamDataToDataSubscriber()
{
    var kafkaPublisher = new KafkaPublisher(new Logger<KafkaPublisher>(new LoggerFactory()));
    var topic = ConfigurationHelper.GetConfiguration().GetValue<string>("KAFKA:TOPIC");
    var folderPath = ConfigurationHelper.GetConfiguration().GetValue<string>("BIN_LOGS_PATH");
    var messageTypes = ConfigurationHelper.GetConfiguration().GetSection("MESSAGE_TYPE").Get<string[]>();
    var orderBookId = ConfigurationHelper.GetConfiguration().GetValue<int>("ORDER_BOOK_ID");
    var containsOrderBookId = "\"OrderBookID\":{\"Value\":" + orderBookId + "}";

    if (!string.IsNullOrEmpty(folderPath))
    {
        var files = Directory.EnumerateFiles(folderPath)
            .Where(file => Path.GetExtension(file).Equals(".pi", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        foreach (var file in files)
        {
            if (string.IsNullOrEmpty(file) || !File.Exists(file))
            {
                Log.Information("The file does not exist.");
                continue;
            }

            using var stream = new StreamReader(file);
            while (await stream.ReadLineAsync() is { } line)
                try
                {
                    var cleanedJson = line.SimpleCleanJsonMessage();

                    if (string.IsNullOrEmpty(cleanedJson))
                    {
                        Log.Information($"Found empty line after cleaning: {line}");
                        continue;
                    }

                    if (!cleanedJson.IsValidJsonMessage())
                    {
                        Log.Information($"Found invalid line after cleaning: {line}");
                        continue;
                    }

                    var stockMessage = JsonConvert.DeserializeObject<StockMessage>(cleanedJson);

                    if (messageTypes != null &&
                        (stockMessage == null || !messageTypes.Contains(stockMessage.MessageType))) continue;

                    if (stockMessage?.MessageType != "T")
                        if (string.IsNullOrEmpty(stockMessage?.Message) ||
                            !stockMessage.Message.Contains(containsOrderBookId, StringComparison.OrdinalIgnoreCase))
                            continue;

                    Log.Information("Processing message: " + cleanedJson);
                    if (topic != null) await kafkaPublisher.PublishAsync(topic, stockMessage);
                    Log.Information(string.Empty);
                }
                catch (JsonException ex)
                {
                    Log.Information($"JSON parsing error: {ex.Message}");
                    Log.Information($"Problematic line: {line}");
                    Log.Information($"Cleaned JSON: {line.CleanJsonMessage()}");
                }
                catch (Exception ex)
                {
                    Log.Information($"Unexpected error: {ex.Message}");
                    Log.Information($"Problematic line: {line}");
                }

            Log.Information("End of file!");
        }
    }

    Log.Information("Press any key to continue . . .");
    Console.ReadKey();
}

#pragma warning disable CS8321 // Local function is declared but never used
async Task StreamDataToSignalRHub()
#pragma warning restore CS8321 // Local function is declared but never used
{
    var redisPublisher = new RedisPublisher();
    var symbols = ConfigurationHelper.GetConfiguration().GetSection("SYMBOLS").Get<string[]>();
    var venue = ConfigurationHelper.GetConfiguration().GetValue<string>("VENUE");
    var count = 0;

    // Main loop to update and publish data
    while (true)
    {
        if (count > 100000) break;

        // Select 5 random symbols from the list
        if (symbols != null)
        {
            var selectedSymbols = symbols.OrderBy(_ => Guid.NewGuid()).Take(5).ToList();

            // Generate random stock data for the selected symbols
            // EQSM or NASDAQ
            var mockData = GenerateRandomStockData(selectedSymbols, venue ?? "NASDAQ");

            // Serialize and publish the data to Redis
            await PublishToRedis(mockData, redisPublisher);
        }

        // Wait for a second before next update
        await Task.Delay(200);
        count++;
    }
}

// Function to generate random stock data
MarketStreamingResponse GenerateRandomStockData(List<string> symbolList, string venue)
{
    var random = new Random();
    var selectedSymbols = symbolList.Select(symbol => new StreamingBody
        {
            Symbol = symbol,
            Venue = venue,
            Price = (100 + random.Next(-50, 51) * 0.01m).ToString("F2"),
            AuctionPrice = (99 + random.Next(-50, 51) * 0.01m).ToString("F2"),
            AuctionVolume = random.Next(100, 10000).ToString(),
            IsProjected = random.Next(0, 2) == 1,
            LastPriceTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Open = (100 + random.Next(-50, 51) * 0.01m).ToString("F2"),
            High24H = (105 + random.Next(-50, 51) * 0.01m).ToString("F2"),
            Low24H = (95 + random.Next(-50, 51) * 0.01m).ToString("F2"),
            PriceChanged = random.Next(-5, 6).ToString("F2"),
            PriceChangedRate = random.Next(-5, 6).ToString("F2") + "%",
            Volume = random.Next(1000, 100000).ToString(),
            Amount = (random.Next(1000, 100000) * random.Next(10, 100)).ToString(),
            TotalAmount = (random.Next(1000, 100000) * random.Next(10, 100)).ToString(),
            TotalAmountK = random.Next(1000, 10000).ToString(),
            TotalVolume = random.Next(1000, 100000).ToString(),
            TotalVolumeK = random.Next(1000, 10000).ToString(),
            Open1 = (100 + random.Next(-50, 51) * 0.01m).ToString("F2"),
            Open2 = (100 + random.Next(-50, 51) * 0.01m).ToString("F2"),
            Ceiling = (150 + random.Next(-50, 51) * 0.01m).ToString("F2"),
            Floor = (50 + random.Next(-50, 51) * 0.01m).ToString("F2"),
            Average = (100 + random.Next(-50, 51) * 0.01m).ToString("F2"),
            AverageBuy = (100 + random.Next(-50, 51) * 0.01m).ToString("F2"),
            AverageSell = (100 + random.Next(-50, 51) * 0.01m).ToString("F2"),
            Aggressor = "Buy",
            PreClose = (100 + random.Next(-50, 51) * 0.01m).ToString("F2"),
            Status = "Open",
            Yield = random.Next(0, 5) + "%",
            PublicTrades = new List<List<object>>
            {
                new() { (100 + random.Next(-50, 51) * 0.01m).ToString("F2"), random.Next(10, 100), "Buy" },
                new() { (100 + random.Next(-50, 51) * 0.01m).ToString("F2"), random.Next(10, 100), "Sell" }
            },
            OrderBook = new StreamingOrderBook
            {
                Bid = new List<List<string>>
                {
                    new() { (100 + random.Next(-50, 51) * 0.01m).ToString("F2"), random.Next(100, 1000).ToString() },
                    new() { (100 + random.Next(-50, 51) * 0.01m).ToString("F2"), random.Next(100, 1000).ToString() }
                },
                Offer = new List<List<string>>
                {
                    new() { (100 + random.Next(-50, 51) * 0.01m).ToString("F2"), random.Next(100, 1000).ToString() },
                    new() { (100 + random.Next(-50, 51) * 0.01m).ToString("F2"), random.Next(100, 1000).ToString() }
                }
            },
            SecurityType = "Equity",
            InstrumentType = "Stock",
            Market = venue,
            LastTrade = (100 + random.Next(-50, 51) * 0.01m).ToString("F2"),
            ToLastTrade = random.Next(0, 10),
            Moneyness = "In the Money",
            MaturityDate = "2023-12-31",
            Multiplier = "1",
            ExercisePrice = (100 + random.Next(-50, 51) * 0.01m).ToString("F2"),
            IntrinsicValue = random.Next(0, 10).ToString("F2"),
            PSettle = (100 + random.Next(-50, 51) * 0.01m).ToString("F2"),
            Poi = "Point of Interest",
            Underlying = symbol,
            Open0 = (100 + random.Next(-50, 51) * 0.01m).ToString("F2"),
            Basis = random.Next(-5, 6).ToString("F2"),
            Settle = (100 + random.Next(-50, 51) * 0.01m).ToString("F2")
        })
        .ToList();

    return new MarketStreamingResponse
    {
        Code = "200",
        Op = "Streaming",
        Message = "Success",
        Response = new StreamingResponse
        {
            Data = selectedSymbols
        }
    };
}

// Publish data to Redis
async Task PublishToRedis(MarketStreamingResponse mockData, RedisPublisher redisPublisher)
{
    var keySpace = ConfigurationHelper.GetConfiguration().GetValue<string>("REDIS:KEY_SPACE");
    var channel = ConfigurationHelper.GetConfiguration().GetValue<string>("REDIS:CHANNEL");

    await redisPublisher.PublishAsync($"{keySpace}{channel}", mockData);
    var mock = JsonConvert.SerializeObject(mockData);

    Log.Information($"{mock}{Environment.NewLine}======================== END ========================");
}
using Microsoft.Extensions.Configuration;
using Pi.SetMarketDataWSS.Infrastructure.Interfaces.Redis;
using Pi.SetMarketDataWSS.DataSubscriber.BidOfferMapper;
using Pi.SetMarketDataWSS.Application.Services;
using Pi.SetMarketDataWSS.Infrastructure.Services.Redis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pi.SetMarketDataWSS.BidOfferServiceTest;

static class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  generate <symbol> <count> - Generate bid/offer items");
            Console.WriteLine("  get <symbol> - Get latest bid/offer array");
            return;
        }

        // Configure Redis connection using IConfiguration
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Redis:Host"] = "localhost",
                ["Redis:Port"] = "6379"
            })
            .Build();

        var redisPublisher = new RedisV2Publisher(configuration);
        var bidOfferService = new BidOfferService(redisPublisher);

        string command = args[0].ToLower();

        switch (command)
        {
            case "generate":
                // if (args.Length != 3 || !int.TryParse(args[2], out int count))
                // {
                //     Console.WriteLine("Invalid arguments for generate command.");
                //     Console.WriteLine("Usage: generate <symbol> <count>");
                //     return;
                // }
                // await GenerateBidOffers(bidOfferService, args[1], count);
                break;

            case "get":
                if (args.Length != 2)
                {
                    Console.WriteLine("Invalid arguments for get command.");
                    Console.WriteLine("Usage: get <symbol>");
                    return;
                }
                await GetLatestBidOffers(bidOfferService, args[1]);
                break;

            default:
                Console.WriteLine("Unknown command. Use 'generate' or 'get'.");
                break;
        }
    }

    private static async Task GenerateBidOffers(BidOfferService bidOfferService, string symbol, int count)
    {
        Random random = new Random();
        string[] sides = { "A", "B" };
        string[] actions = { "N", "C", "D" };

        Console.WriteLine($"Generating {count} bid/offer items for {symbol}...");

        for (int i = 0; i < count; i++)
        {
            long sequence = random.Next(1, 1000000);
            string randomSide = sides[random.Next(sides.Length)];
            string randomAction = actions[random.Next(actions.Length)];
            int randomLevel = random.Next(10, 11);
            decimal randomPrice = random.Next(90, 119);

            await bidOfferService.AddBidOfferItem(
                symbol: symbol,
                sequence: sequence,
                side: randomSide,
                action: randomAction,
                level: randomLevel,
                price: randomPrice,
                quantity: 1.5m
            );
            Console.WriteLine($"[{i}] Sequence: {sequence}, Symbol: {symbol}, Side: {randomSide}, Level: {randomLevel}, Action: {randomAction}, Price: {randomPrice}");
            await Task.Delay(10);
        }
    }

    private static async Task GetLatestBidOffers(BidOfferService bidOfferService, string symbol)
    {
        var (bids, offers) = await bidOfferService.GetLatestBidOfferArray(symbol);
        Console.WriteLine("Current Bid/Offer Array:");
        Console.WriteLine($"Bids: {JsonSerializer.Serialize(bids, new JsonSerializerOptions { WriteIndented = true })}");
        Console.WriteLine($"Offers: {JsonSerializer.Serialize(offers, new JsonSerializerOptions { WriteIndented = true })}");
    }
}
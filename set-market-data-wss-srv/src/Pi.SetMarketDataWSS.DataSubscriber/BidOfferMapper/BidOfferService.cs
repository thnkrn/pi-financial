using System.Text.Json;
using System.Text.Json.Serialization;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchMessageWrapper;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchOrderBookMapper;
using Pi.SetMarketDataWSS.Application.Utils;
using Pi.SetMarketDataWSS.Infrastructure.Interfaces.Redis;

namespace Pi.SetMarketDataWSS.DataSubscriber.BidOfferMapper;

public interface IBidOfferService
{
    Task<bool> AddBidOfferItem(
        string symbol,
        long sequence,
        string side,
        string action,
        int level,
        decimal price,
        decimal quantity);

    Task<(List<List<string>> Bids, List<List<string>> Offers)> GetLatestBidOfferArray(string symbol);
    Task<(List<List<string>> Bids, List<List<string>> Offers)> GetLatestBidOfferArrayCache(string symbol);

    Task<bool> AddBidOfferItemsFromPriceLevelUpdates(string symbol,
        long sequence,
        long timestampNanoseconds,
        List<PriceLevelUpdate> updates,
        int decimalsInPriceValue);

    Task ResetBidOfferAsync(OrderBookStateMessageWrapper? orderBookStateMessage);
}

public class BidOfferService : IBidOfferService
{
    private const string BidOfferRawKeyspace = "v2-bidoffer-raw";
    private const string BidOfferArrayKeyspace = "v2-bidoffer-array";
    private readonly IRedisV2Publisher _redisPublisher;
    private const int MaxLevel = 10;

    private const string PreOpen1E = "PRE-OPEN1_E";
    private const string PreOpenE = "PRE-OPEN_E";
    private const string PreMorningD = "PRE-MORNING_D";
    private const string PreNightD = "PRE-NIGHT_D";
    private const string PreOpenL = "PRE-OPEN_L";

    private static readonly HashSet<string> States = new(StringComparer.OrdinalIgnoreCase)
        { PreOpen1E, PreOpenE, PreMorningD, PreNightD, PreOpenL };

    /// <summary>
    /// </summary>
    /// <param name="redisPublisher"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public BidOfferService(IRedisV2Publisher redisPublisher)
    {
        _redisPublisher = redisPublisher ?? throw new ArgumentNullException(nameof(redisPublisher));
    }

    public async Task<bool> AddBidOfferItem(
        string symbol,
        long sequence,
        string side,
        string action,
        int level,
        decimal price,
        decimal quantity)
    {
        var bidOfferItem = new BidOfferItem
        {
            O = symbol,
            Sq = sequence,
            S = side,
            A = action,
            L = level,
            P = price,
            Q = quantity
        };

        var key = $"{symbol}::{BidOfferRawKeyspace}";
        var member = JsonSerializer.Serialize(bidOfferItem);

        try
        {
            // Store the order data with sequence as score in a sorted set
            await _redisPublisher.AddSortedSetAsync(key, member, sequence);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> AddBidOfferItemsFromPriceLevelUpdates(
        string symbol,
        long sequence,
        long timestampNanoseconds,
        List<PriceLevelUpdate> updates,
        int decimalsInPriceValue)
    {
        // Example:
        // - symbol = 655452

        // [
        //     {'level_update_action': b'D', 'side': 'B', 'level': 1, 'price': -2147483648, 'quantity': 0, 'number_of_delete': 2}, 
        //     {'level_update_action': b'N', 'side': 'B', 'level': 1, 'price': 148000, 'quantity': 13182000, 'number_of_delete': 0}, 
        //     {'level_update_action': b'N', 'side': 'B', 'level': 10, 'price': 3000, 'quantity': 200000, 'number_of_delete': 0}
        // ]
        // - decimal in price value = 5
        try
        {
            var batch = _redisPublisher.CreateTransaction();
            var key = $"{symbol}::{BidOfferRawKeyspace}";

            var bidOfferItems = new List<BidOfferItem>();
            foreach (var update in updates)
            {
                decimal price;
                try
                {
                    // Data manipulation
                    price = decimal.Parse(update.Price.Value.ToString()
                        .FormatDecimals(decimalsInPriceValue));
                }
                catch
                {
                    price = decimal.Parse(update.Price.Value.ToString());
                }

                var bidOfferItem = new BidOfferItem
                {
                    O = symbol,
                    Sq = sequence,
                    S = update.Side,
                    A = update.UpdateAction,
                    L = update.Level,
                    P = price,
                    Q = update.Quantity,
                    Nd = update.NumberOfDeletes
                };
                bidOfferItems.Add(bidOfferItem);
            }

            var updateMembers = JsonSerializer.Serialize(bidOfferItems);
            batch.SortedSetAdd(key, updateMembers, timestampNanoseconds);

            await _redisPublisher.ExecuteTransactionAsync(batch);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task ResetBidOfferAsync(OrderBookStateMessageWrapper? orderBookStateMessage)
    {
        if (orderBookStateMessage == null)
        {
            return;
        }

        if (!States.Contains(orderBookStateMessage.StateName?.Value ?? string.Empty))
        {
            return;
        }

        var redisKey = $"{orderBookStateMessage.OrderBookId}::{BidOfferRawKeyspace}";

        await _redisPublisher.SortedSetRemoveRangeByRankAsync(redisKey, 0, -1);
    }

    public async Task<(List<List<string>> Bids, List<List<string>> Offers)> GetLatestBidOfferArray(string symbol)
    {
        var rawBidOfferKey = $"{symbol}::{BidOfferRawKeyspace}";
        var bidOfferArrayKey = $"{symbol}::{BidOfferArrayKeyspace}";

        var (bids, offers) = await GetLatestBidOfferArrayCache(symbol);

        // Get all entries from the sorted set
        var entries = await _redisPublisher.GetSortedSetWithScoresAsync(rawBidOfferKey, 0, -1);

        // Process each order in sequence
        var batch = _redisPublisher.CreateTransaction();
        foreach (var entry in entries)
        {
            var updateMembers = JsonSerializer.Deserialize<List<BidOfferItem>>(entry.Element.ToString());
            (bids, offers) = CalculateBidOfferArray(bids, offers, updateMembers, MaxLevel);
        }

        batch.HashSet(bidOfferArrayKey, "B", JsonSerializer.Serialize(bids));
        batch.HashSet(bidOfferArrayKey, "A", JsonSerializer.Serialize(offers));

        await _redisPublisher.ExecuteTransactionAsync(batch);

        // Cleanup old entries
        // TODO: donot remove it yet!!
        // var bidOfferArrayTask = _redisPublisher.HashGetAllAsync(bidOfferArrayKey);

        var cleanupTask = CleanupBidAskOrders(symbol);
        await Task.WhenAll(cleanupTask);

        return (bids, offers);
    }

    public async Task<(List<List<string>> Bids, List<List<string>> Offers)> GetLatestBidOfferArrayCache(string symbol)
    {
        var bidOfferArrayKey = $"{symbol}::{BidOfferArrayKeyspace}";
        if (!await _redisPublisher.KeyExistsAsync(bidOfferArrayKey))
        {
            return InitBidOfferArray(MaxLevel);
        }

        var bidOfferHash = await _redisPublisher.HashGetAllAsync(bidOfferArrayKey);
        var bids = JsonSerializer.Deserialize<List<List<string>>>(bidOfferHash["B"])?.ToList();
        var offers = JsonSerializer.Deserialize<List<List<string>>>(bidOfferHash["A"])?.ToList();

        return (bids, offers);
    }

    private async Task CleanupBidAskOrders(string symbol, int keepLast = 100)
    {
        var redisKey = $"{symbol}::{BidOfferRawKeyspace}";
        var totalEntries = await _redisPublisher.SortedSetLengthAsync(redisKey);

        if (totalEntries > keepLast)
        {
            var entriesToRemove = totalEntries - keepLast;
            await _redisPublisher.SortedSetRemoveRangeByRankAsync(redisKey, 0, entriesToRemove - 1);
        }
    }

    public static (List<List<string>> Bids, List<List<string>> Offers) InitBidOfferArray(int maxLevel)
    {
        var bids = new List<List<string>>();
        var offers = new List<List<string>>();
        for (var i = 0; i < maxLevel; i++)
        {
            bids.Add(new List<string> { "0.00", "0.00" });
            offers.Add(new List<string> { "0.00", "0.00" });
        }

        return (bids, offers);
    }

    public static (List<List<string>> bids, List<List<string>> offers) CalculateBidOfferArray(List<List<string>> bids,
        List<List<string>> offers, List<BidOfferItem> updateMembers, int maxLevel)
    {
        foreach (var update in updateMembers)
        {
            var level = update.L - 1;
            var action = update.A;
            var side = update.S;
            var price = update.P;
            var quantity = update.Q;

            if (side == "B")
            {
                if (action == "N")
                {
                    bids?.Insert(level, new List<string> { price.ToString("F2"), quantity.ToString("F2") });
                }
                else if (action == "C")
                {
                    bids[level] = new List<string> { price.ToString("F2"), quantity.ToString("F2") };
                }
                else if (action == "D")
                {
                    bids.RemoveRange(level, update.Nd);
                }
            }
            else if (side == "A")
            {
                if (action == "N")
                {
                    offers?.Insert(level, new List<string> { price.ToString("F2"), quantity.ToString("F2") });
                }
                else if (action == "C")
                {
                    offers[level] = new List<string> { price.ToString("F2"), quantity.ToString("F2") };
                }
                else if (action == "D")
                {
                    offers.RemoveRange(level, update.Nd);
                }
            }

            if (bids.Count < maxLevel)
            {
                bids = bids.Concat(Enumerable.Repeat(new List<string> { "0.00", "0.00" }, maxLevel - bids.Count))
                    .ToList();
            }

            if (offers.Count < maxLevel)
            {
                offers = offers.Concat(Enumerable.Repeat(new List<string> { "0.00", "0.00" }, maxLevel - offers.Count))
                    .ToList();
            }

            // if (side == "B")
            // {
            //     Console.WriteLine($"\n");
            //     Console.WriteLine($"[{update.Sq}] Level: {update.L} Action: {update.A} Side: {update.S} Price: {update.P} Quantity: {update.Q}");
            //     Console.WriteLine($"Update: {JsonSerializer.Serialize(update)}");
            //     Console.WriteLine($"Bids: {JsonSerializer.Serialize(bids, new JsonSerializerOptions { WriteIndented = true })}");
            //     // Console.WriteLine($"Offers: {JsonSerializer.Serialize(offers)}");
            //     Console.WriteLine("Press any key to continue...");
            //     Console.ReadKey();
            // }
        }

        if (bids.Count > maxLevel)
        {
            bids = bids.Slice(0, maxLevel).ToList();
        }

        if (offers.Count > maxLevel)
        {
            offers = offers.Slice(0, maxLevel).ToList();
        }

        return (bids, offers);
    }

    private static bool ValidateBidOfferSort(List<List<string>> bids, List<List<string>> offers)
    {
        // Check bids (should be descending)
        for (int i = 0; i < bids.Count - 1; i++)
        {
            var currentPrice = decimal.Parse(bids[i][0]);
            var nextPrice = decimal.Parse(bids[i + 1][0]);

            if (currentPrice > 0 && currentPrice < nextPrice && nextPrice != 0m)
            {
                return false;
            }
        }

        // Check offers (should be ascending)
        for (int i = 0; i < offers.Count - 1; i++)
        {
            var currentPrice = decimal.Parse(offers[i][0]);
            var nextPrice = decimal.Parse(offers[i + 1][0]);

            if (currentPrice > 0 && currentPrice > nextPrice && nextPrice != 0m)
            {
                return false;
            }
        }

        return true;
    }
}

public class BidOfferItem
{
    [JsonPropertyName("o")] public required string O { get; set; } // symbol
    [JsonPropertyName("sq")] public long Sq { get; set; } // sequence
    [JsonPropertyName("s")] public required string S { get; set; } // side
    [JsonPropertyName("a")] public required string A { get; set; } // action
    [JsonPropertyName("l")] public int L { get; set; } // level
    [JsonPropertyName("p")] public decimal P { get; set; } // price
    [JsonPropertyName("q")] public decimal Q { get; set; } // quantity
    [JsonPropertyName("nd")] public int Nd { get; set; } // number of delete
}
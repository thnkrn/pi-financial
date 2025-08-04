using Pi.SetMarketDataWSS.Application.Interfaces.ItchMapper;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchMessageWrapper;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchOrderBookMapper;

namespace Pi.SetMarketDataWSS.Application.Services.ItchMapper;

public class ItchPublicTradeMapperService : IItchPublicTradeMapper
{
    private const int MaxPublicTradeCount = 50;

    public PublicTradeResult Map(TradeTickerMessageWrapper message, PublicTrade[]? cachePublicTrade)
    {
        var newPublicTrade = new PublicTrade(message.DealDateTime.ToUnixTimestampSeconds(), message.DealDateTime,
            message.Aggressor,
            message.Quantity, message.Price);

        // Create a HashSet to check for duplicates (using Nanos as the unique identifier)
        // Add the new trade's Nanos to the HashSet
        var existingNanosValues = new HashSet<long>
        {
            newPublicTrade.Nanos
        };

        // Create list with new trade as first element
        var updatedTrades = new List<PublicTrade>(MaxPublicTradeCount) { newPublicTrade };

        // Only add cached trades if they are not duplicates of the new trade
        if (cachePublicTrade is { Length: > 0 })
            foreach (var cachedTrade in cachePublicTrade)
            {
                // Skip if this trade's Nanos is already in our HashSet
                if (!existingNanosValues.Add(cachedTrade.Nanos))
                    continue;

                updatedTrades.Add(cachedTrade);

                // Early exit if we've reached the maximum capacity
                if (updatedTrades.Count >= MaxPublicTradeCount)
                    break;
            }

        // Return the result (no need for Take() since we control the count in the loop above)
        return new PublicTradeResult(updatedTrades.ToArray());
    }
}
using Pi.SetMarketDataWSS.Application.Interfaces.ItchMapper;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchMessageWrapper;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;
using Pi.SetMarketDataWSS.Application.Utils;
using Pi.SetMarketDataWSS.Domain.Entities;

namespace Pi.SetMarketDataWSS.Application.Services.ItchMapper;

public class ItchTickSizeTableEntryMapperService : IItchTickSizeTableEntryMapperService
{
    public TickSizeTableEntry Map(ItchMessage message, TickSizeTableEntry? cachedTickSizeTableEntry, InstrumentDetail? instrumentDetail)
    {
        if (message is not TickSizeTableMessageWrapper tickSizeTableMessage)
        {
            throw new ArgumentException("Invalid message type", nameof(message));
        }
        var decimalInPrice = instrumentDetail?.Decimals ?? 0;

        var tickSizeTableEntry = cachedTickSizeTableEntry ?? new TickSizeTableEntry();

        tickSizeTableEntry.OrderBookId = tickSizeTableMessage.OrderBookId?.Value;
        tickSizeTableEntry.TickSize = GetDecimal(tickSizeTableMessage.TickSize, decimalInPrice);
        tickSizeTableEntry.PriceFrom = GetDecimal(tickSizeTableMessage.PriceFrom, decimalInPrice);
        tickSizeTableEntry.PriceTo = GetDecimal(tickSizeTableMessage.PriceTo, decimalInPrice);

        return tickSizeTableEntry;
    }
    private static decimal GetDecimal(Price price, int decimalInPrice)
    {
        if (price == null)
        {
            return 0m;
        }

        return price.Value / (decimal)Math.Pow(10, decimalInPrice);
    }
}

using Pi.SetMarketData.Application.Interfaces.ItchMapper;
using Pi.SetMarketData.Application.Models.ItchMessageWrapper;
using Pi.SetMarketData.Application.Services.Models.ItchParser;
using Pi.SetMarketData.Application.Services.Types.ItchParser;
using Pi.SetMarketData.Domain.Entities;


namespace Pi.SetMarketData.Application.Services.ItchMapper;

public class ItchTickSizeTableEntryMapperService : IItchTickSizeTableEntryMapperService
{
    public Instrument? MapToInstrument(ItchMessage message)
    {
        if(message == null)
        { 
            return null; 
        }

        var instrument = new Instrument();
        var tickSizeTableEntry = (TickSizeTableMessageWrapper)message;

        instrument.OrderBookId = tickSizeTableEntry.OrderBookId?.Value ?? 0;
        instrument.TradingUnit = tickSizeTableEntry.TickSize?.Value.ToString();

        return instrument;
    }
}

using Pi.SetMarketDataWSS.Application.Services.Models.ItchMessageWrapper;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;
using Pi.SetMarketDataWSS.Domain.Entities;

namespace Pi.SetMarketDataWSS.Application.Interfaces.ItchMapper;

public interface IItchTickSizeTableEntryMapperService
{
    TickSizeTableEntry Map(ItchMessage message, TickSizeTableEntry? cachedTickSizeTableEntry, InstrumentDetail? instrumentDetail);
}

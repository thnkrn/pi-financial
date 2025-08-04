using Pi.SetMarketData.Application.Services.Models.ItchParser;
using Pi.SetMarketData.Domain.Entities;

namespace Pi.SetMarketData.Application.Interfaces.ItchMapper;

public interface IItchTickSizeTableEntryMapperService
{
    Domain.Entities.Instrument? MapToInstrument(ItchMessage message);
}

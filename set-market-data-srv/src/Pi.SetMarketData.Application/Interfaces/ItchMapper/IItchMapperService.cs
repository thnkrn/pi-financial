using Pi.SetMarketData.Application.Services.Models.ItchParser;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Domain.Models.DataProcessing;

namespace Pi.SetMarketData.Application.Interfaces.ItchMapper;

public interface IItchMapperService
{
    IEnumerable<DataProcessingResult?> MapToDatabase
    (
        ItchMessage message,
        Domain.Entities.OrderBook? storedData,
        bool? exchangeServer,
        string? venue,
        MarketDirectory? marketDirectory
    );
}
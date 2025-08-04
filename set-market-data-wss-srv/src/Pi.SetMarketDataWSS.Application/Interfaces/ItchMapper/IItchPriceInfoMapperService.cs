using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;
using Pi.SetMarketDataWSS.Domain.Entities;

namespace Pi.SetMarketDataWSS.Application.Interfaces.ItchMapper;

public interface IItchPriceInfoMapperService
{
    public PriceInfo Map(ItchMessage message, object? cachedPriceInfo, object? cachedPriceInfoQUpper, string orderBookStateStatus = "");
}
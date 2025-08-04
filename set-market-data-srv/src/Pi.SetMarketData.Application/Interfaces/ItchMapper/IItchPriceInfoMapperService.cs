using Pi.SetMarketData.Application.Services.Models.ItchParser;

namespace Pi.SetMarketData.Application.Interfaces.ItchMapper;

public interface IItchPriceInfoMapperService
{
    public Domain.Entities.PriceInfo? Map(ItchMessage? message);
}
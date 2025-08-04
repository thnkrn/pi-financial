using Pi.MarketData.Domain.Models;
using Pi.MarketData.Domain.Models.Request;

namespace Pi.MarketData.Application.Interfaces;

public interface ICuratedFilterService 
{
    string? GetDomain(FiltersRequestPayload requestPayload);
    string? GetDomain(HomeInstrumentPayload requestPayload);
    string GetDomain(MarketFiltersRequest requestPayload);
}
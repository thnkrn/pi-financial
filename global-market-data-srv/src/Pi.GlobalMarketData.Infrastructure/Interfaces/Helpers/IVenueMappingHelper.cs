namespace Pi.GlobalMarketData.Infrastructure.Interfaces.Helpers;

public interface IVenueMappingHelper
{
    Task<string> GetExchangeNameFromVenueMapping(string? requestExchange);
}

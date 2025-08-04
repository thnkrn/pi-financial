using Pi.GlobalMarketData.Infrastructure.Interfaces.EntityCacheService;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Helpers;

namespace Pi.GlobalMarketData.Infrastructure.Helpers;

public class VenueMappingHelper : IVenueMappingHelper
{
    private readonly IEntityCacheService _entityCacheService;

    public VenueMappingHelper(IEntityCacheService entityCacheService)
    {
        _entityCacheService = entityCacheService;
    }

    public async Task<string> GetExchangeNameFromVenueMapping(string? requestExchange)
    {
        string exchange = requestExchange?.ToLower() ?? string.Empty;

        var geVenueMapping = await _entityCacheService.GetGeVenueMappingByExchangeIdMs(exchange);

        if (geVenueMapping != null)
        {
            exchange = geVenueMapping.VenueCode;
        }

        return exchange;
    }
}

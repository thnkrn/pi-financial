using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Queries.MarketFilters;

namespace Pi.SetMarketData.Application.Interfaces.MarketFilters;

public abstract class PostMarketFiltersRequestAbstractHandler
    : RequestHandler<PostMarketFiltersRequest, PostMarketFiltersResponse>;

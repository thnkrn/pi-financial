using Pi.GlobalMarketData.Application.Abstractions;
using Pi.GlobalMarketData.Application.Queries;

namespace Pi.GlobalMarketData.Application.Interfaces.MarketFilters;

public abstract class PostMarketFiltersRequestAbstractHandler
    : RequestHandler<PostMarketFiltersRequest, PostMarketFiltersResponse>;

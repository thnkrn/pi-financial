using MassTransit.Mediator;
using Pi.SetMarketData.Domain.Models.Request;

namespace Pi.SetMarketData.Application.Queries.MarketFilters;

public record PostMarketFiltersRequest(MarketFiltersRequest Data) : Request<PostMarketFiltersResponse>;

using MassTransit.Mediator;
using Pi.GlobalMarketData.Domain.Models.Request;

namespace Pi.GlobalMarketData.Application.Queries;

public record PostMarketProfileOverViewRequest(MarketProfileOverviewRequest data)
    : Request<PostMarketProfileOverviewResponse>;

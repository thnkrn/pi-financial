using MassTransit.Mediator;
using Pi.SetMarketData.Domain.Models.Request;

namespace Pi.SetMarketData.Application.Queries.MarketTimelineRendered;

public record PostMarketTimelineRenderedRequest(MarketTimelineRenderedRequest Data)
    : Request<PostMarketTimelineRenderedResponse>;

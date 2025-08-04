using MassTransit.Mediator;
using Pi.SetMarketData.Domain.Models.Request;

namespace Pi.SetMarketData.Application.Queries.MarketIndicator;

public record PostMarketIndicatorRequest(MarketIndicatorRequest Data)
    : Request<PostMarketIndicatorResponse>;

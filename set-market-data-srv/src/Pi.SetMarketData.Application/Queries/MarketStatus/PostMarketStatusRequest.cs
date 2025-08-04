using MassTransit.Mediator;
using Pi.SetMarketData.Application.Queries.MarketStatus;
using Pi.SetMarketData.Domain.Models.Request;

namespace Pi.SetMarketData.Application.Queries.MarketTicker;

public record PostMarketStatusRequest(MarketStatusRequest Data) : Request<PostMarketStatusResponse>;

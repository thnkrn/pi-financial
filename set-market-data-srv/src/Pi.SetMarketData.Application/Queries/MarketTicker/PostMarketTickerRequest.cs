using MassTransit.Mediator;
using Pi.SetMarketData.Domain.Models.Request;

namespace Pi.SetMarketData.Application.Queries.MarketTicker;

public record PostMarketTickerRequest(MarketTickerRequest Data) : Request<PostMarketTickerResponse>;

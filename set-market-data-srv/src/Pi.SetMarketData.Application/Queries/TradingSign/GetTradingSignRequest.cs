using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Queries.TradingSign;

public record GetTradingSignRequest : Request<GetTradingSignResponse>;
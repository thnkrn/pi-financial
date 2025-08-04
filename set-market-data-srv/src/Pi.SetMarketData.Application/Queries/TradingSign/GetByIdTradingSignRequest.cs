using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Queries.TradingSign;

public record GetByIdTradingSignRequest(string id) : Request<GetByIdTradingSignResponse>;
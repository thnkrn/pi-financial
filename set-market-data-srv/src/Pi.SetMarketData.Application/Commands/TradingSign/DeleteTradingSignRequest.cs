using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.TradingSign;

public record DeleteTradingSignRequest(string id) : Request<DeleteTradingSignResponse>;
using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.TradingSign;

public record UpdateTradingSignRequest(string id, Domain.Entities.TradingSign TradingSign) : Request<UpdateTradingSignResponse>;
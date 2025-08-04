using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.TradingSign;

public record CreateTradingSignRequest(Domain.Entities.TradingSign TradingSign) : Request<CreateTradingSignResponse>;
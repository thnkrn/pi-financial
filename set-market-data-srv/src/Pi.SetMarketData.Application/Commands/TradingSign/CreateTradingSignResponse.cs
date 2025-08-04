namespace Pi.SetMarketData.Application.Commands.TradingSign;

public record CreateTradingSignResponse(bool Success, Domain.Entities.TradingSign? CreatedTradingSign = null);
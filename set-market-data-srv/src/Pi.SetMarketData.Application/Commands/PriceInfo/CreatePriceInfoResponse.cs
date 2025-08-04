namespace Pi.SetMarketData.Application.Commands.PriceInfo;

public record CreatePriceInfoResponse(bool Success, Domain.Entities.PriceInfo? CreatedPriceInfo = null);
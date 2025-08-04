namespace Pi.SetMarketData.Application.Commands.PriceInfo;

public record UpdatePriceInfoResponse(bool Success, Domain.Entities.PriceInfo? UpdatedPriceInfo = null);
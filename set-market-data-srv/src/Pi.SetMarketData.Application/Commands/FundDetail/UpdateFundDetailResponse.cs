namespace Pi.SetMarketData.Application.Commands.FundDetail;

public record UpdateFundDetailResponse(bool Success, Domain.Entities.FundDetail? UpdatedFundDetail = null);
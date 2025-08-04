namespace Pi.SetMarketData.Application.Commands.FundDetail;

public record CreateFundDetailResponse(bool Success, Domain.Entities.FundDetail? CreatedFundDetail = null);
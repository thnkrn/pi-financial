namespace Pi.SetMarketData.Application.Queries.PriceInfo;

public record GetPriceInfoResponse(List<Domain.Entities.PriceInfo> Data);
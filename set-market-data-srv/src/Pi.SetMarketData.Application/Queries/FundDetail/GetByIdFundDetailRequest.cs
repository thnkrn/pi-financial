using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Queries.FundDetail;

public record GetByIdFundDetailRequest(string id) : Request<GetByIdFundDetailResponse>;
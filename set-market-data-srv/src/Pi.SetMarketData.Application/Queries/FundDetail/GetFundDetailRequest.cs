using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Queries.FundDetail;

public record GetFundDetailRequest : Request<GetFundDetailResponse>;
using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.FundDetail;

public record DeleteFundDetailRequest(string id) : Request<DeleteFundDetailResponse>;
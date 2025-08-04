using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.FundDetail;

public record UpdateFundDetailRequest(string id, Domain.Entities.FundDetail FundDetail) : Request<UpdateFundDetailResponse>;
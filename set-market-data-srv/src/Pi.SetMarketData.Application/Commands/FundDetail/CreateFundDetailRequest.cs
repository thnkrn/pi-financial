using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.FundDetail;

public record CreateFundDetailRequest(Domain.Entities.FundDetail FundDetail) : Request<CreateFundDetailResponse>;
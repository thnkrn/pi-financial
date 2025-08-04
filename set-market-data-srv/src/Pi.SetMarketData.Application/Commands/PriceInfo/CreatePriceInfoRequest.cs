using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.PriceInfo;

public record CreatePriceInfoRequest(Domain.Entities.PriceInfo PriceInfo) : Request<CreatePriceInfoResponse>;
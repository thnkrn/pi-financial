using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.PriceInfo;

public record UpdatePriceInfoRequest(string id, Domain.Entities.PriceInfo PriceInfo) : Request<UpdatePriceInfoResponse>;
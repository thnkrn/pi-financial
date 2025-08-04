using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.PriceInfo;

public record DeletePriceInfoRequest(string id) : Request<DeletePriceInfoResponse>;
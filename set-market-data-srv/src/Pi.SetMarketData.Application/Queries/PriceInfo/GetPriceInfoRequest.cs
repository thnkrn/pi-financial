using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Queries.PriceInfo;

public record GetPriceInfoRequest : Request<GetPriceInfoResponse>;
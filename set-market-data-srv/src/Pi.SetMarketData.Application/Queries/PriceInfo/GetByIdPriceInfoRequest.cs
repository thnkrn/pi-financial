using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Queries.PriceInfo;

public record GetByIdPriceInfoRequest(string id) : Request<GetByIdPriceInfoResponse>;
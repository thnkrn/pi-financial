using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Queries.InstrumentDetail;

public record GetInstrumentDetailRequest : Request<GetInstrumentDetailResponse>;
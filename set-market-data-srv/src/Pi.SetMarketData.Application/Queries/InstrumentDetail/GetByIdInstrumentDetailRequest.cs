using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Queries.InstrumentDetail;

public record GetByIdInstrumentDetailRequest(string id) : Request<GetByIdInstrumentDetailResponse>;
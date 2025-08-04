using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Queries.Instrument;

public record GetByIdInstrumentRequest(string id) : Request<GetByIdInstrumentResponse>;
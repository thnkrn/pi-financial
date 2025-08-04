using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Queries.Instrument;

public record GetInstrumentRequest : Request<GetInstrumentResponse>;
using MassTransit.Mediator;

namespace Pi.GlobalMarketDataRealTime.Application.Queries.GeInstrument;

public record GetByIdGeInstrumentRequest(string Id) : Request<GetByIdGeInstrumentResponse>;
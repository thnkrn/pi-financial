using MassTransit.Mediator;

namespace Pi.GlobalMarketDataRealTime.Application.Queries.GeInstrument;

public record GetGeInstrumentRequest : Request<GetGeInstrumentResponse>;
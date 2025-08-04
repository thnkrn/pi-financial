using MassTransit.Mediator;

namespace Pi.GlobalMarketDataWSS.Application.Queries.GeInstrument;

public record GetGeInstrumentRequest : Request<GetGeInstrumentResponse>;
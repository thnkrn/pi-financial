using MassTransit.Mediator;

namespace Pi.GlobalMarketDataWSS.Application.Queries.GeInstrument;

public record GetByIdGeInstrumentRequest(string id) : Request<GetByIdGeInstrumentResponse>;
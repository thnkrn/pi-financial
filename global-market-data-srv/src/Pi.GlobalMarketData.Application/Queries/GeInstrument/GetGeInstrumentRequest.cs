using MassTransit.Mediator;

namespace Pi.GlobalMarketData.Application.Queries;

public record GetGeInstrumentRequest : Request<GetGeInstrumentResponse>;

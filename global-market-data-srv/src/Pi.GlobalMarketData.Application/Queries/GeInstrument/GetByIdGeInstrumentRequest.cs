using MassTransit.Mediator;

namespace Pi.GlobalMarketData.Application.Queries;

public record GetByIdGeInstrumentRequest(string id) : Request<GetByIdGeInstrumentResponse>;

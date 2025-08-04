using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.Instrument;

public record UpdateInstrumentRequest(string id, Domain.Entities.Instrument Instrument) : Request<UpdateInstrumentResponse>;
using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.Instrument;

public record CreateInstrumentRequest(Domain.Entities.Instrument Instrument) : Request<CreateInstrumentResponse>;
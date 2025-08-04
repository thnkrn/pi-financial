using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.Instrument;

public record DeleteInstrumentRequest(string id) : Request<DeleteInstrumentResponse>;
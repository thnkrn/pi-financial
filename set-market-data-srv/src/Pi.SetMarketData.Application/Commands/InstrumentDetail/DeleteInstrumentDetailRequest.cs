using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.InstrumentDetail;

public record DeleteInstrumentDetailRequest(string id) : Request<DeleteInstrumentDetailResponse>;
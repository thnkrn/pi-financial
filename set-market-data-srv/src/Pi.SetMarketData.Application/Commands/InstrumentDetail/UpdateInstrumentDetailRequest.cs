using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.InstrumentDetail;

public record UpdateInstrumentDetailRequest(string id, Domain.Entities.InstrumentDetail InstrumentDetail) : Request<UpdateInstrumentDetailResponse>;
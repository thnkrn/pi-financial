using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.InstrumentDetail;

public record CreateInstrumentDetailRequest(Domain.Entities.InstrumentDetail InstrumentDetail) : Request<CreateInstrumentDetailResponse>;
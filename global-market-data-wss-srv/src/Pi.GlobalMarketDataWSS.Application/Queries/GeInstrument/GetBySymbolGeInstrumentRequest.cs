using MassTransit.Mediator;

namespace Pi.GlobalMarketDataWSS.Application.Queries.GeInstrument;

public record GetBySymbolGeInstrumentRequest(string symbol, string investmentType)
    : Request<GetBySymbolGeInstrumentResponse>;
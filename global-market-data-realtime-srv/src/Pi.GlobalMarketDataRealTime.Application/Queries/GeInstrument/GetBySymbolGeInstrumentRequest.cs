using MassTransit.Mediator;

namespace Pi.GlobalMarketDataRealTime.Application.Queries.GeInstrument;

public record GetBySymbolGeInstrumentRequest(string Symbol, string InvestmentType)
    : Request<GetBySymbolGeInstrumentResponse>;
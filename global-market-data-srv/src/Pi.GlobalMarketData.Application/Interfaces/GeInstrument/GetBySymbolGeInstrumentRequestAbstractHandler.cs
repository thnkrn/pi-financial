using Pi.GlobalMarketData.Application.Abstractions;
using Pi.GlobalMarketData.Application.Queries;

namespace Pi.GlobalMarketData.Application.Interfaces.GeInstrument;

public abstract class GetBySymbolGeInstrumentRequestAbstractHandler
    : RequestHandler<GetBySymbolGeInstrumentRequest, GetBySymbolGeInstrumentResponse>;

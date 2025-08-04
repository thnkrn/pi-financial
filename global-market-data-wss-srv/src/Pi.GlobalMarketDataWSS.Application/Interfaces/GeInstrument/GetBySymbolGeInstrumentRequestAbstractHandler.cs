using Pi.GlobalMarketDataWSS.Application.Abstractions;
using Pi.GlobalMarketDataWSS.Application.Queries.GeInstrument;

namespace Pi.GlobalMarketDataWSS.Application.Interfaces.GeInstrument;

public abstract class
    GetBySymbolGeInstrumentRequestAbstractHandler : RequestHandler<GetBySymbolGeInstrumentRequest,
    GetBySymbolGeInstrumentResponse>;
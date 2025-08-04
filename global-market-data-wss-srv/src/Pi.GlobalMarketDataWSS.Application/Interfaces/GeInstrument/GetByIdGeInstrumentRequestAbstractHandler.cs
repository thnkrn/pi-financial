using Pi.GlobalMarketDataWSS.Application.Abstractions;
using Pi.GlobalMarketDataWSS.Application.Queries.GeInstrument;

namespace Pi.GlobalMarketDataWSS.Application.Interfaces.GeInstrument;

public abstract class
    GetByIdGeInstrumentRequestAbstractHandler : RequestHandler<GetByIdGeInstrumentRequest, GetByIdGeInstrumentResponse>;
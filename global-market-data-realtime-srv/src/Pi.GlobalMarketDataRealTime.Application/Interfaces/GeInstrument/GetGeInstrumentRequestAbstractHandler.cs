using Pi.GlobalMarketDataRealTime.Application.Abstractions;
using Pi.GlobalMarketDataRealTime.Application.Queries.GeInstrument;

namespace Pi.GlobalMarketDataRealTime.Application.Interfaces.GeInstrument;

public abstract class
    GetGeInstrumentRequestAbstractHandler : RequestHandler<GetGeInstrumentRequest, GetGeInstrumentResponse>;
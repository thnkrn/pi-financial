using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Queries.Instrument;

namespace Pi.SetMarketData.Application.Interfaces.Instrument;

public abstract class GetByIdInstrumentRequestAbstractHandler: RequestHandler<GetByIdInstrumentRequest, GetByIdInstrumentResponse>;
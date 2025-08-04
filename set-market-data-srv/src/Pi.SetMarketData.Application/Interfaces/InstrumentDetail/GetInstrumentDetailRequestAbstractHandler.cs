using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Queries.InstrumentDetail;

namespace Pi.SetMarketData.Application.Interfaces.InstrumentDetail;

public abstract class GetInstrumentDetailRequestAbstractHandler: RequestHandler<GetInstrumentDetailRequest, GetInstrumentDetailResponse>;
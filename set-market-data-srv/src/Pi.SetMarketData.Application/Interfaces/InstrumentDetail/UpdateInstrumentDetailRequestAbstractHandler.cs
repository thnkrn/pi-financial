using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Commands.InstrumentDetail;

namespace Pi.SetMarketData.Application.Interfaces.InstrumentDetail;

public abstract class UpdateInstrumentDetailRequestAbstractHandler : RequestHandler<UpdateInstrumentDetailRequest, UpdateInstrumentDetailResponse>;
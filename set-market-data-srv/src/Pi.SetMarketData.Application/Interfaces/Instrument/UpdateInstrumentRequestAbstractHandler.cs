using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Commands.Instrument;

namespace Pi.SetMarketData.Application.Interfaces.Instrument;

public abstract class UpdateInstrumentRequestAbstractHandler : RequestHandler<UpdateInstrumentRequest, UpdateInstrumentResponse>;
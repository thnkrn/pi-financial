using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Commands.Instrument;

namespace Pi.SetMarketData.Application.Interfaces.Instrument;

public abstract class CreateInstrumentRequestAbstractHandler: RequestHandler<CreateInstrumentRequest, CreateInstrumentResponse>;
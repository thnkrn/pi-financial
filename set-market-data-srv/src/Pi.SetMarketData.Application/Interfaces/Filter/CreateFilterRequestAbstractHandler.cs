using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Commands.Filter;

namespace Pi.SetMarketData.Application.Interfaces.Filter;

public abstract class CreateFilterRequestAbstractHandler: RequestHandler<CreateFilterRequest, CreateFilterResponse>;
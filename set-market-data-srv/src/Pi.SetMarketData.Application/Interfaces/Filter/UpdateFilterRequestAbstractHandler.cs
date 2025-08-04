using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Commands.Filter;

namespace Pi.SetMarketData.Application.Interfaces.Filter;

public abstract class UpdateFilterRequestAbstractHandler : RequestHandler<UpdateFilterRequest, UpdateFilterResponse>;
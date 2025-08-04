using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Commands.MarketStatus;

namespace Pi.SetMarketData.Application.Interfaces.MarketStatus;

public abstract class UpdateMarketStatusRequestAbstractHandler : RequestHandler<UpdateMarketStatusRequest, UpdateMarketStatusResponse>;
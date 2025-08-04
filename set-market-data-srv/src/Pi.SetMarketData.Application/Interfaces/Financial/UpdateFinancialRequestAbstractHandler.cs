using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Commands.Financial;

namespace Pi.SetMarketData.Application.Interfaces.Financial;

public abstract class UpdateFinancialRequestAbstractHandler : RequestHandler<UpdateFinancialRequest, UpdateFinancialResponse>;
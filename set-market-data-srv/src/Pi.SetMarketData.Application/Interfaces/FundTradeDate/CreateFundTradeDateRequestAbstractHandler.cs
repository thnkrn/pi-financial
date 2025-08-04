using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Commands.FundTradeDate;

namespace Pi.SetMarketData.Application.Interfaces.FundTradeDate;

public abstract class CreateFundTradeDateRequestAbstractHandler: RequestHandler<CreateFundTradeDateRequest, CreateFundTradeDateResponse>;
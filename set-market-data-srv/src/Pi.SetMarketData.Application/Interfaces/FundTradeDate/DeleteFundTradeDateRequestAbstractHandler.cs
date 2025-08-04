using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Commands.FundTradeDate;

namespace Pi.SetMarketData.Application.Interfaces.FundTradeDate;

public abstract class DeleteFundTradeDateRequestAbstractHandler: RequestHandler<DeleteFundTradeDateRequest, DeleteFundTradeDateResponse>;
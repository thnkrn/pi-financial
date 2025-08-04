using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Queries.FundTradeDate;

namespace Pi.SetMarketData.Application.Interfaces.FundTradeDate;

public abstract class GetByIdFundTradeDateRequestAbstractHandler: RequestHandler<GetByIdFundTradeDateRequest, GetByIdFundTradeDateResponse>;
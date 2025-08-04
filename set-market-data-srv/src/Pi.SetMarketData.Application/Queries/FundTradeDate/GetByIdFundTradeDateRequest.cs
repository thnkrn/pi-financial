using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Queries.FundTradeDate;

public record GetByIdFundTradeDateRequest(string id) : Request<GetByIdFundTradeDateResponse>;
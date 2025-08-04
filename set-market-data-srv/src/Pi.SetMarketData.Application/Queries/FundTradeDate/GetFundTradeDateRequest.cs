using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Queries.FundTradeDate;

public record GetFundTradeDateRequest : Request<GetFundTradeDateResponse>;
using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.FundTradeDate;

public record DeleteFundTradeDateRequest(string id) : Request<DeleteFundTradeDateResponse>;
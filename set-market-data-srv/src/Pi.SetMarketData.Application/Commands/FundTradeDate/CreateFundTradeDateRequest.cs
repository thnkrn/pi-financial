using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.FundTradeDate;

public record CreateFundTradeDateRequest(Domain.Entities.FundTradeDate FundTradeDate) : Request<CreateFundTradeDateResponse>;
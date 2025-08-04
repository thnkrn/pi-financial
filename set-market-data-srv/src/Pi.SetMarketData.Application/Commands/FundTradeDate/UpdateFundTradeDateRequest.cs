using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.FundTradeDate;

public record UpdateFundTradeDateRequest(string id, Domain.Entities.FundTradeDate FundTradeDate) : Request<UpdateFundTradeDateResponse>;
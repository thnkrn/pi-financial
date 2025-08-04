using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Commands.FundPerformance;

namespace Pi.SetMarketData.Application.Interfaces.FundPerformance;

public abstract class UpdateFundPerformanceRequestAbstractHandler : RequestHandler<UpdateFundPerformanceRequest, UpdateFundPerformanceResponse>;
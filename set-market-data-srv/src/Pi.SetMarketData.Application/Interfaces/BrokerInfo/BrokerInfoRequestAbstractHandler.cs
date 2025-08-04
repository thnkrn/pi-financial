using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Commands.BrokerInfo;

namespace Pi.SetMarketData.Application.Interfaces.MarketFilters;

public abstract class BrokerInfoRequestAbstractHandler
    : RequestHandler<PostBrokerInfoRequest, PostBrokerInfoResponse>;
using Pi.GlobalMarketData.Application.Abstractions;
using Pi.GlobalMarketData.Application.Queries;

namespace Pi.GlobalMarketData.Application.Interfaces.MarketStatus;

public abstract class PostMarketStatusRequestAbstactHandler : RequestHandler<PostMarketStatusRequest, PostMarketStatusResponse>;


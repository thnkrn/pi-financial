using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Queries.PublicTrade;

namespace Pi.SetMarketData.Application.Interfaces.PublicTrade;

public abstract class GetByIdPublicTradeRequestAbstractHandler: RequestHandler<GetByIdPublicTradeRequest, GetByIdPublicTradeResponse>;
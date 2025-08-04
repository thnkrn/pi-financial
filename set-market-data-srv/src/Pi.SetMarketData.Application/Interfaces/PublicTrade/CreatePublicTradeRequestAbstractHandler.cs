using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Commands.PublicTrade;

namespace Pi.SetMarketData.Application.Interfaces.PublicTrade;

public abstract class CreatePublicTradeRequestAbstractHandler: RequestHandler<CreatePublicTradeRequest, CreatePublicTradeResponse>;
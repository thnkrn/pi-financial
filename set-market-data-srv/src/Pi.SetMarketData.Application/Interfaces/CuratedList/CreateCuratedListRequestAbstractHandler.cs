using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Commands;

namespace Pi.SetMarketData.Application.Interfaces;

public abstract class CreateCuratedListRequestAbstractHandler: RequestHandler<CreateCuratedListRequest, CreateCuratedListResponse>;
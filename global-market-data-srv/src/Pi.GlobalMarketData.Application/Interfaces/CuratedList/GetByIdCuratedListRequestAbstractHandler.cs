using Pi.GlobalMarketData.Application.Abstractions;
using Pi.GlobalMarketData.Application.Queries;

namespace Pi.GlobalMarketData.Application.Interfaces.CuratedList;

public abstract class GetByIdCuratedListRequestAbstractHandler
    : RequestHandler<GetByIdCuratedListRequest, GetByIdCuratedListResponse>;

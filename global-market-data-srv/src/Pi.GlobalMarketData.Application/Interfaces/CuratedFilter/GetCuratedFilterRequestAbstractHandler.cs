using Pi.GlobalMarketData.Application.Abstractions;
using Pi.GlobalMarketData.Application.Queries;

namespace Pi.GlobalMarketData.Application.Interfaces;

public abstract class GetCuratedFilterRequestAbstractHandler: RequestHandler<GetCuratedFilterRequest, GetCuratedFilterResponse>;
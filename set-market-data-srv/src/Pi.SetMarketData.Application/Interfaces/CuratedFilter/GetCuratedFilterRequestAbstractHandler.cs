using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Queries;

namespace Pi.SetMarketData.Application.Interfaces;

public abstract class GetCuratedFilterRequestAbstractHandler: RequestHandler<GetCuratedFilterRequest, GetCuratedFilterResponse>;
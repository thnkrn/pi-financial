using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Queries;

namespace Pi.SetMarketData.Application.Interfaces;

public abstract class GetByIdCuratedFilterRequestAbstractHandler: RequestHandler<GetByIdCuratedFilterRequest, GetByIdCuratedFilterResponse>;
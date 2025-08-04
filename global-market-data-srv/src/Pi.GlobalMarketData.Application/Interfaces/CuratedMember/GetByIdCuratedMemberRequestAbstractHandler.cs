using Pi.GlobalMarketData.Application.Abstractions;
using Pi.GlobalMarketData.Application.Queries;

namespace Pi.GlobalMarketData.Application.Interfaces;

public abstract class GetByIdCuratedMemberRequestAbstractHandler: RequestHandler<GetByIdCuratedMemberRequest, GetByIdCuratedMemberResponse>;
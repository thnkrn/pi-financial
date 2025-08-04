using Pi.GlobalMarketData.Application.Abstractions;
using Pi.GlobalMarketData.Application.Commands;

namespace Pi.GlobalMarketData.Application.Interfaces;

public abstract class CreateCuratedMemberRequestAbstractHandler: RequestHandler<CreateCuratedMemberRequest, CreateCuratedMemberResponse>;
using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Queries.CorporateAction;

namespace Pi.SetMarketData.Application.Interfaces.CorporateAction;

public abstract class GetCorporateActionRequestAbstractHandler: RequestHandler<GetCorporateActionRequest, GetCorporateActionResponse>;
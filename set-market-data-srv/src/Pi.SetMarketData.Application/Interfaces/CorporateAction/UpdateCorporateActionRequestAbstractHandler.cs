using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Commands.CorporateAction;

namespace Pi.SetMarketData.Application.Interfaces.CorporateAction;

public abstract class UpdateCorporateActionRequestAbstractHandler : RequestHandler<UpdateCorporateActionRequest, UpdateCorporateActionResponse>;
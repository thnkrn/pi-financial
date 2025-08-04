namespace Pi.SetMarketData.Application.Commands.CorporateAction;

public record UpdateCorporateActionResponse(bool Success, Domain.Entities.CorporateAction? UpdatedCorporateAction = null);
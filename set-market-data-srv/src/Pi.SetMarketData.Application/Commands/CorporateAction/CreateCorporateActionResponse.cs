namespace Pi.SetMarketData.Application.Commands.CorporateAction;

public record CreateCorporateActionResponse(bool Success, Domain.Entities.CorporateAction? CreatedCorporateAction = null);
namespace Pi.SetMarketData.Application.Commands.Financial;

public record UpdateFinancialResponse(bool Success, Domain.Entities.Financial? UpdatedFinancial = null);
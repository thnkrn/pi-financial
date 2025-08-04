namespace Pi.SetMarketData.Application.Commands.Financial;

public record CreateFinancialResponse(bool Success, Domain.Entities.Financial? CreatedFinancial = null);
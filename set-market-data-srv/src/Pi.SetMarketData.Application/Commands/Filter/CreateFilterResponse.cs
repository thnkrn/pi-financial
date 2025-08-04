namespace Pi.SetMarketData.Application.Commands.Filter;

public record CreateFilterResponse(bool Success, Domain.Entities.Filter? CreatedFilter = null);
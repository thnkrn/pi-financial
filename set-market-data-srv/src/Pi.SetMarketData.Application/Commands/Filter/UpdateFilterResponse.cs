namespace Pi.SetMarketData.Application.Commands.Filter;

public record UpdateFilterResponse(bool Success, Domain.Entities.Filter? UpdatedFilter = null);
namespace Pi.SetMarketData.Application.Commands.Indicator;

public record UpdateIndicatorResponse(bool Success, Domain.Entities.Indicator? UpdatedIndicator = null);
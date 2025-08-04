namespace Pi.SetMarketData.Application.Commands.Indicator;

public record CreateIndicatorResponse(bool Success, Domain.Entities.Indicator? CreatedIndicator = null);
namespace Pi.SetMarketData.Application.Commands.Intermission;

public record UpdateIntermissionResponse(bool Success, Domain.Entities.Intermission? UpdatedIntermission = null);
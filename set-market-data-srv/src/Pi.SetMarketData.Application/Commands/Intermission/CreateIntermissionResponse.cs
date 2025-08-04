namespace Pi.SetMarketData.Application.Commands.Intermission;

public record CreateIntermissionResponse(bool Success, Domain.Entities.Intermission? CreatedIntermission = null);
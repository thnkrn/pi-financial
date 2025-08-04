using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Services;

public record ValidationResult
{
    public bool Success { get; init; }
    public string ErrorCode { get; init; } = "";
    public string ErrorMessage { get; init; } = "";
}

public interface IValidationService
{
    bool IsOutsideWorkingHour(Product product, Channel channel, DateTime currentDateTime, out ValidationResult validationResult);
}
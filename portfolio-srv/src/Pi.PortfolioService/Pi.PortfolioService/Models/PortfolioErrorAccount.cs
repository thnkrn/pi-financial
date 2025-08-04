namespace Pi.PortfolioService.Models;

public record PortfolioErrorAccount(
    string AccountType,
    string AccountId,
    string ErrorMessage,
    string AccountNoForDisplay
);

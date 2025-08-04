namespace Pi.PortfolioService.Models;

public record GeneralError(
    string AccountType,
    string Error
);
namespace Pi.SetService.Application.Models.AccountSummaries;

public interface ICreditBalanceSummary
{
    decimal Mr { get; init; }
    decimal EquityValue { get; init; }
    decimal ExcessEquity { get; init; }
    decimal Liabilities { get; init; }
    decimal MarginLoan { get; }
}

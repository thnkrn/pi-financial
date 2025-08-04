using Pi.Common.Domain.AggregatesModel.ProductAggregate;

namespace Pi.User.Application.Models;

public record TradingAccountWithProductInfo
{
    public required string TradingAccountNo { get; init; }
    public required Product Product { get; init; }
    public decimal CreditLine { get; init; }
    public required string CreditLineCurrency { get; init; }
    public DateOnly? CreditLineEffectiveDate { get; init; }
    public DateOnly? CreditLineEndDate { get; init; }
    public string? MarketingId { get; init; }
    public DateOnly? AccountOpeningDate { get; init; }
    public string? AccountStatus { get; init; }
};
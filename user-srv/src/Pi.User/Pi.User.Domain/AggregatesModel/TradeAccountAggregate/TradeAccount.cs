using Pi.Common.SeedWork;
using Pi.User.Domain.AggregatesModel.ExternalAccountAggregate;

namespace Pi.User.Domain.AggregatesModel.TradeAccountAggregate;

public class TradeAccount(Guid id) : Entity<Guid>(id), IAggregateRoot, IAuditableEntity
{
    public required string AccountNumber { get; init; }
    public AccountType AccountType { get; init; }
    public required string AccountTypeCode { get; init; }
    public required string ExchangeMarketId { get; init; }
    public required string AccountStatus { get; set; }
    public decimal CreditLine { get; init; }
    public string CreditLineCurrency { get; set; } = "THB";
    public DateOnly? EffectiveDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? MarketingId { get; set; }
    public string? SaleLicense { get; set; }
    public DateOnly? OpenDate { get; init; }
    public required string UserAccountId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // relation
    public ICollection<ExternalAccount> ExternalAccounts { get; } = new List<ExternalAccount>();
}
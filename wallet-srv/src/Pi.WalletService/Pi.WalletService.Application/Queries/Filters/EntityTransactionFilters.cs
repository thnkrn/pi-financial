using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Queries.Filters;

public abstract class EntityTransactionFilters
{
    public Channel? Channel { get; set; }
    public string? State { get; set; }
    public TransactionStatus? Status { get; set; }
    public string? BankCode { get; set; }
    public string? BankAccountNo { get; set; }
    public string? CustomerCode { get; set; }
    public string? AccountCode { get; set; }
    public string? TransactionNo { get; set; }
    public DateTime? EffectiveDateFrom { get; set; }
    public DateTime? EffectiveDateTo { get; set; }
    public DateTime? CreatedAtFrom { get; set; }
    public DateTime? CreatedAtTo { get; set; }
}

public abstract class EntityTransactionV2Filters
{
    public Channel? Channel { get; set; }
    public string? State { get; set; }
    public Status? Status { get; set; }
    public Product[]? Product { get; set; }
    public string? TransactionNo { get; set; }
    public string? UserId { get; set; }
    public string? BankCode { get; set; }
    public string? BankName { get; set; }
    public string? BankAccountNo { get; set; }
    public string? CustomerCode { get; set; }
    public string? AccountCode { get; set; }
    public DateTime? EffectiveDateFrom { get; set; }
    public DateTime? EffectiveDateTo { get; set; }
    public DateTime? CreatedAtFrom { get; set; }
    public DateTime? CreatedAtTo { get; set; }
    public string[]? NotStates { get; set; }
}

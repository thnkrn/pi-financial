using System.Linq.Expressions;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;

public abstract class TransactionFilters<T> : IQueryFilter<T> where T : class, ITransactionState
{
    public Channel? Channel { get; set; }
    public Product? Product { get; set; }
    public Product[]? Products { get; set; }
    public string? State { get; set; }
    public string[]? States { get; set; }
    public string? BankCode { get; set; }
    public string? BankAccountNo { get; set; }
    public string? CustomerCode { get; set; }
    public string? AccountCode { get; set; }
    public string? TransactionNo { get; set; }
    public DateTime? CreatedAtFrom { get; set; }
    public DateTime? CreatedAtTo { get; set; }
    public string? UserId { get; set; }

    public virtual List<Expression<Func<T, bool>>> DefaultExpressions()
    {
        return new List<Expression<Func<T, bool>>>()
        {
            q => q.TransactionNo != null,
        };
    }

    public virtual List<Expression<Func<T, bool>>> GetExpressions()
    {
        var result = DefaultExpressions();

        if (Channel != null) result.Add(q => q.Channel == Channel);

        if (Product != null) result.Add(q => q.Product == Product);

        if (Products != null) result.Add(q => Products.Contains(q.Product));

        if (!string.IsNullOrEmpty(State)) result.Add(q => q.CurrentState == State);

        if (States != null) result.Add(q => States.Contains(q.CurrentState));

        if (!string.IsNullOrEmpty(BankCode)) result.Add(q => q.BankCode == BankCode);

        if (!string.IsNullOrEmpty(BankAccountNo))
        {
            result.Add(q => q.BankAccountNo != null && q.BankAccountNo.Replace("-", "") == BankAccountNo.Replace("-", ""));
        }

        if (!string.IsNullOrEmpty(CustomerCode)) result.Add(q => q.CustomerCode == CustomerCode);

        if (!string.IsNullOrEmpty(AccountCode)) result.Add(q => q.AccountCode == AccountCode);

        if (!string.IsNullOrEmpty(TransactionNo)) result.Add(q => q.TransactionNo == TransactionNo);

        if (CreatedAtFrom != null) result.Add(q => q.CreatedAt >= CreatedAtFrom);

        if (CreatedAtTo != null) result.Add(q => q.CreatedAt <= CreatedAtTo);

        if (UserId != null) result.Add(q => q.UserId == UserId);

        return result;
    }
}

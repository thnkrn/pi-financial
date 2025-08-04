using System.Linq.Expressions;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;

public class GlobalTransferStateFilter
{
    public string? State { get; set; }
    public string[]? States { get; set; }
    public string[]? NotStates { get; set; }
    public string? CustomerCode { get; set; }
    public string? TransactionNo { get; set; }
    public TransactionType? TransactionType { get; set; }
    public DateTime? CreatedAtFrom { get; set; }
    public DateTime? CreatedAtTo { get; set; }
    public string? UserId { get; set; }
    public List<Expression<Func<GlobalWalletTransferState, bool>>> GetExpressions()
    {
        var result = new List<Expression<Func<GlobalWalletTransferState, bool>>>();

        if (!string.IsNullOrEmpty(State)) result.Add(q => q.CurrentState == State);

        if (States != null) result.Add(q => States.Contains(q.CurrentState));

        if (NotStates != null) result.Add(q => !NotStates.Contains(q.CurrentState));

        if (!string.IsNullOrEmpty(CustomerCode)) result.Add(q => q.CustomerCode == CustomerCode);

        if (!string.IsNullOrEmpty(TransactionNo)) result.Add(q => q.TransactionNo == TransactionNo);

        if (CreatedAtFrom != null) result.Add(q => q.CreatedAt >= CreatedAtFrom);

        if (CreatedAtTo != null) result.Add(q => q.CreatedAt <= CreatedAtTo);

        if (UserId != null) result.Add(q => q.UserId == UserId);

        if (TransactionType != null) result.Add(q => q.TransactionType == TransactionType);

        return result;
    }
}
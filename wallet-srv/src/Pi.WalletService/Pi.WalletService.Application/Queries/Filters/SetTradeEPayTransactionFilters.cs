using System.Linq.Expressions;
using Pi.WalletService.Application.Factories;
using Pi.WalletService.Domain;
using Pi.WalletService.Domain.AggregatesModel.CashAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;

namespace Pi.WalletService.Application.Queries.Filters;

public class SetTradeEPayTransactionFilters : EntityTransactionV2Filters, IQueryFilter<CashDepositState>
{
    public List<Expression<Func<CashDepositState, bool>>> GetExpressions()
    {
        var result = new List<Expression<Func<CashDepositState, bool>>>()
        {
            q => !string.IsNullOrEmpty(q.TransactionNo) && q.Channel == IntegrationEvents.AggregatesModel.Channel.SetTrade
        };

        if (Product is { Length: > 0 }) result.Add(q => Product.Contains(q.Product));

        if (!string.IsNullOrEmpty(BankName)) result.Add(q => q.BankName != null && q.BankName.ToLower().Equals(BankName.ToLower()));

        if (!string.IsNullOrEmpty(CustomerCode)) result.Add(q => q.CustomerCode == CustomerCode);

        if (!string.IsNullOrEmpty(AccountCode)) result.Add(q => q.AccountCode == AccountCode);

        if (!string.IsNullOrEmpty(TransactionNo)) result.Add(q => q.TransactionNo == TransactionNo);

        if (Status != null)
        {
            var depositStates = EntityFactory.GetSetTradeEPayStatus(Status.Value);
            result.Add(q => depositStates.Contains(q.CurrentState));
        }

        if (CreatedAtFrom != null) result.Add(q => q.CreatedAt >= CreatedAtFrom);

        if (CreatedAtTo != null) result.Add(q => q.CreatedAt <= CreatedAtTo);

        return result;
    }
}
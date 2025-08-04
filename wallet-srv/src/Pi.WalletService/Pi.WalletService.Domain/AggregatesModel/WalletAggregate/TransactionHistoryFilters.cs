using System.Linq.Expressions;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Domain.AggregatesModel.WalletAggregate;

public class TransactionHistoryFilters
{
    public TransactionHistoryFilters(
        Channel? channel,
        Product? product,
        int? errorCode,
        string? bankName,
        string? bankAccountNo,
        string? customerCode,
        string? accountCode,
        string? customerName,
        string? transactionNo)
    {
        Channel = channel;
        Product = product;
        ErrorCode = errorCode;
        BankName = bankName;
        BankAccountNo = bankAccountNo;
        CustomerCode = customerCode;
        AccountCode = accountCode;
        CustomerName = customerName;
        TransactionNo = transactionNo;
    }

    public Channel? Channel { get; }
    public Product? Product { get; }
    public int? ErrorCode { get; }
    public int[]? ErrorCodes { get; set; }
    public string? BankName { get; }
    public string? BankAccountNo { get; }
    public string? CustomerCode { get; }
    public string? CustomerName { get; }
    public string? AccountCode { get; }
    public string? TransactionNo { get; }

    public List<Expression<Func<TransactionHistory, bool>>> GetExpressions()
    {
        var result = new List<Expression<Func<TransactionHistory, bool>>>();

        if (Channel != null) result.Add(q => q.Channel == Channel);

        if (Product != null) result.Add(q => q.Product == Product);

        if (!string.IsNullOrEmpty(BankName)) result.Add(q => q.BankName == BankName);

        if (!string.IsNullOrEmpty(BankAccountNo)) result.Add(q => q.BankAccountNo == BankAccountNo);

        if (!string.IsNullOrEmpty(CustomerCode)) result.Add(q => q.CustomerCode == CustomerCode);

        if (!string.IsNullOrEmpty(CustomerName)) result.Add(q => q.CustomerName == CustomerName);

        if (!string.IsNullOrEmpty(AccountCode)) result.Add(q => q.AccountCode == AccountCode);

        if (!string.IsNullOrEmpty(TransactionNo)) result.Add(q => q.TransactionNo == TransactionNo);

        return result;
    }
}

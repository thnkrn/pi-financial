using System.ComponentModel.DataAnnotations;
using Pi.Common.SeedWork;
using Pi.User.Domain.AggregatesModel.UserInfoAggregate;

namespace Pi.User.Domain.AggregatesModel.BankAccountAggregate;

public class BankAccount : Entity<Guid>, IAuditableEntity, IAggregateRoot
{
    public BankAccount(
        Guid id,
        Guid userId,
        string accountNo,
        string accountNoHash,
        string accountName,
        string accountNameHash,
        string bankCode,
        string? bankBranchCode) : base(id)
    {
        UserId = userId;
        AccountNo = accountNo;
        AccountNoHash = accountNoHash;
        AccountName = accountName;
        AccountNameHash = accountNameHash;
        BankCode = bankCode;
        BankBranchCode = bankBranchCode;
    }
    public Guid UserId { get; init; }

    [Encrypted] public string AccountNo { get; private set; }

    public string AccountNoHash { get; private set; }

    [Encrypted] public string AccountName { get; private set; }

    public string AccountNameHash { get; private set; }
    public string BankCode { get; private set; }
    public string? BankBranchCode { get; private set; }
    public UserInfo User { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public void Update(
        string accountNo,
        string accountNoHash,
        string accountName,
        string accountNameHash,
        string bankCode,
        string? bankBranchCode)
    {
        AccountNo = accountNo;
        AccountNoHash = accountNoHash;
        AccountName = accountName;
        AccountNameHash = accountNameHash;
        BankCode = bankCode;
        BankBranchCode = bankBranchCode;
    }
}
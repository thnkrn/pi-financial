using Pi.Common.SeedWork;

namespace Pi.User.Domain.AggregatesModel.BankAccountAggregate;

public class BankAccountV2(Guid id) : Entity<Guid>(id), IAggregateRoot
{
    public required string AccountNo { get; set; }
    public required string HashedAccountNo { get; set; }
    public required string AccountName { get; set; }
    public required string BankCode { get; set; }
    public required string BranchCode { get; set; }
    public string? PaymentToken { get; set; }
    public DateTime? AtsEffectiveDate { get; set; }
    public Status Status { get; set; }
    public Guid UserId { get; init; }
}
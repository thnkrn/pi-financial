using Pi.Common.SeedWork;
using Pi.User.Domain.SeedWork;
namespace Pi.User.Domain.AggregatesModel.UserInfoAggregate;

public class TradingAccount : Entity, IAuditableEntity
{
    public TradingAccount(
        string tradingAccountId,
        string acctCode
    )
    {
        TradingAccountId = tradingAccountId;
        AcctCode = acctCode;
    }

    public Guid Id { get; private set; }
    public string TradingAccountId { get; private set; }
    public string AcctCode { get; set; }
    public Guid? UserInfoId { get; }
    public UserInfo? UserInfo { get; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}


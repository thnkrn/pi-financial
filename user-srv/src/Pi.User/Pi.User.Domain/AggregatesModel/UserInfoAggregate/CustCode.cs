using Pi.Common.SeedWork;
using Pi.User.Domain.SeedWork;
namespace Pi.User.Domain.AggregatesModel.UserInfoAggregate;

public class CustCode : Entity, IAuditableEntity
{
    public CustCode(string customerCode)
    {
        CustomerCode = customerCode;
    }

    public Guid Id { get; private set; }
    public string CustomerCode { get; private set; }
    public Guid? UserInfoId { get; private set; }
    public UserInfo? UserInfo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
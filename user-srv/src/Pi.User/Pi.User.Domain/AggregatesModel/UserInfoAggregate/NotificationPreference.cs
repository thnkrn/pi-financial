using Pi.Common.SeedWork;
using Pi.User.Domain.SeedWork;

namespace Pi.User.Domain.AggregatesModel.UserInfoAggregate;

public class NotificationPreference : Entity, IAuditableEntity
{
    public NotificationPreference(bool important, bool order, bool portfolio, bool wallet,
        bool market)
    {
        Important = important;
        Order = order;
        Portfolio = portfolio;
        Wallet = wallet;
        Market = market;
    }

    public Guid Id { get; private set; }
    public bool Important { get; private set; }
    public bool Order { get; private set; }
    public bool Portfolio { get; private set; }
    public bool Wallet { get; private set; }
    public bool Market { get; private set; }
    public Guid DeviceForeignKey { get; }
    public Guid? UserInfoId { get; }
    public UserInfo? UserInfo { get; }

    public void Update(
        bool? important,
        bool? order,
        bool? portfolio,
        bool? wallet,
        bool? market)
    {
        if (important != null)
        {
            Important = (bool)important;
        }

        if (order != null)
        {
            Order = (bool)order;
        }

        if (portfolio != null)
        {
            Portfolio = (bool)portfolio;
        }

        if (wallet != null)
        {
            Wallet = (bool)wallet;
        }

        if (market != null)
        {
            Market = (bool)market;
        }
    }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
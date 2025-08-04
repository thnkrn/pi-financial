using Pi.ActivityService.IntegrationEvents;
using Pi.BackofficeService.Application.Commands.User;
using Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;
using Pi.BackofficeService.Domain.AggregateModels.User;

namespace Pi.BackofficeService.Application.Factories;

public static class EntityFactory
{
    public static CreateActivityEvent NewActivityEvent(Guid userId, string description, string activityType, Guid? correlationId = null, string? refId = null)
    {
        return new CreateActivityEvent(
            description,
            new Service("BackOffice"),
            new ActivityType(activityType))
        {
            CorrelationId = correlationId,
            UserId = userId.ToString(),
            RefId = refId
        };
    }

    public static User NewUser(UserUpdateOrCreateRequest request)
    {
        return new User(Guid.NewGuid(), request.IamUserId, request.FirstName, request.LastName, request.Email)
        {
            CreatedAt = DateTime.Now
        };
    }

    public static Machine? NewMachine(TransactionType transactionType)
    {
        return transactionType switch
        {
            TransactionType.Deposit => Machine.Deposit,
            TransactionType.Withdraw => Machine.Withdraw,
            _ => null,
        };
    }

    public static ProductType NewProductType(Product product)
    {
        switch (product)
        {
            case Product.GlobalEquity:
                return ProductType.GlobalEquity;
            case Product.Cash:
            case Product.CashBalance:
            case Product.Margin:
            case Product.Funds:
            case Product.TFEX:
                return ProductType.ThaiEquity;
            case Product.Crypto:
            default:
                throw new ArgumentOutOfRangeException(nameof(product), product, null);
        }
    }
}

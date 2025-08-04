using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Models;

namespace Pi.GlobalEquities.Repositories.Models;

public static class ExtensionMethods
{
    public static IAccount ToDomainModel(this AccountEntity accountEntity)
    {
        if (accountEntity == null)
            return null;

        return new Account
        {
            Id = accountEntity.Id,
            UserId = accountEntity.UserId,
            CustCode = accountEntity.CustCode,
            TradingAccountNo = accountEntity.TradingAccountNo,
            VelexaAccount = accountEntity.VelexaAccount,
            UpdatedAt = accountEntity.UpdatedAt
        };
    }

    public static AccountEntity ToDbModel(this IAccount account)
    {
        if (account == null)
            return null;

        return new AccountEntity
        {
            Id = account.Id,
            UserId = account.UserId,
            CustCode = account.CustCode,
            TradingAccountNo = account.TradingAccountNo,
            VelexaAccount = account.GetProviderAccount(Provider.Velexa),
            UpdatedAt = account.UpdatedAt
        };
    }

    public static IOrder ToDomainModel(this OrderEntity orderEntity)
    {
        if (orderEntity == null)
            return null;

        return new Order
        {
            Id = orderEntity.Id,
            GroupId = orderEntity.GroupId,
            UserId = orderEntity.UserId,
            AccountId = orderEntity.AccountId,
            Venue = orderEntity.Venue,
            Symbol = orderEntity.Symbol,
            OrderType = orderEntity.OrderType,
            Side = orderEntity.Side,
            Duration = orderEntity.Duration,
            Quantity = orderEntity.Quantity,
            LimitPrice = orderEntity.LimitPrice,
            StopPrice = orderEntity.StopPrice,
            Fills = orderEntity.Fills,
            Status = GetOrderStatus(orderEntity.Status),
            ProviderInfo = orderEntity.ProviderInfo.ToDomainModel(),
            Channel = orderEntity.Channel,
            CreatedAt = orderEntity.CreatedAt,
            UpdatedAt = orderEntity.CreatedAt
        };
    }

    public static OrderEntity ToDbModel(this IOrder order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        return new OrderEntity
        {
            Id = order.Id,
            GroupId = order.GroupId,
            UserId = order.UserId,
            AccountId = order.AccountId,
            Venue = order.Venue,
            Symbol = order.Symbol,
            OrderType = order.OrderType,
            Side = order.Side,
            Duration = order.Duration,
            Quantity = order.Quantity,
            LimitPrice = order.LimitPrice,
            StopPrice = order.StopPrice,
            Fills = order.Fills,
            Status = order.Status.ToString(),
            ProviderInfo = order.ProviderInfo.ToDbModel(),
            Channel = order.Channel,
            CreatedAt = order.CreatedAt != default ? order.CreatedAt : DateTime.UtcNow,
            UpdatedAt = order.UpdatedAt
        };
    }

    public static ProviderInfo ToDomainModel(this ProviderInfoEntity providerInfoEntity)
    {
        if (providerInfoEntity == null)
            return null;

        return new ProviderInfo
        {
            ProviderName = providerInfoEntity.ProviderName,
            AccountId = providerInfoEntity.AccountId,
            OrderId = providerInfoEntity.OrderId,
            ModificationId = providerInfoEntity.ModificationId,
            Status = providerInfoEntity.Status,
            StatusReason = providerInfoEntity.StatusReason,
            CreatedAt = providerInfoEntity.CreatedAt,
            ModifiedAt = providerInfoEntity.ModifiedAt
        };
    }

    public static ProviderInfoEntity ToDbModel(this ProviderInfo providerInfo)
    {
        if (providerInfo == null)
            return null;

        return new ProviderInfoEntity
        {
            ProviderName = providerInfo.ProviderName,
            AccountId = providerInfo.AccountId,
            OrderId = providerInfo.OrderId,
            ModificationId = providerInfo.ModificationId,
            Status = providerInfo.Status,
            StatusReason = providerInfo.StatusReason,
            CreatedAt = providerInfo.CreatedAt,
            ModifiedAt = providerInfo.ModifiedAt
        };
    }

    public static WorkerJob<T> ToDomainModel<T>(this WorkerJobEntity<T> jobEntity)
    {
        if (jobEntity == null)
            return null;

        return new WorkerJob<T>
        {
            Name = jobEntity.Name,
            Data = jobEntity.Data
        };
    }

    public static WorkerJobEntity<T> ToDbModel<T>(this WorkerJob<T> job)
    {
        if (job == null)
            return null;

        return new WorkerJobEntity<T>
        {
            Name = job.Name,
            Data = job.Data
        };
    }

    private static OrderStatus GetOrderStatus(string status)
    {
        return status switch
        {
            "Queue" or "Queued" => OrderStatus.Queued,
            "Processing" => OrderStatus.Processing,
            "PartiallyMatched" => OrderStatus.PartiallyMatched,
            "Matched" => OrderStatus.Matched,
            "Canceled" or "Cancelled" => OrderStatus.Cancelled,
            "Rejected" => OrderStatus.Rejected,
            _ => OrderStatus.Unknown
        };
    }
}

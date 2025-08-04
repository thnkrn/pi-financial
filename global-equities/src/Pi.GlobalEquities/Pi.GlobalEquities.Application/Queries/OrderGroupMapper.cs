using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Models;

namespace Pi.GlobalEquities.Application.Queries;

public static class OrderGroupMapper
{
    public static IEnumerable<IOrder> MapOrdersByGroupIds(IEnumerable<IOrder> orders, IEnumerable<TransactionItem>? transactions = null)
    {
        var transByOrderId = transactions?.Where(t => !string.IsNullOrWhiteSpace(t.OrderId))
            .GroupBy(t => t.OrderId)
            .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);

        var results = orders
            .GroupBy(x => x.GroupId)
            .SelectMany(group =>
            {
                if (string.IsNullOrWhiteSpace(group.Key))
                    return GetOrdersWithTransactions(group, transByOrderId);

                var takeProfitOrder = group.FirstOrDefault(z => z.OrderType == OrderType.TakeProfit);
                var stopLossOrder = group.FirstOrDefault(z => z.OrderType == OrderType.StopLoss);
                if (takeProfitOrder != null && stopLossOrder != null)
                    return new[] { GetGroupOrderTpSl(takeProfitOrder, stopLossOrder, transByOrderId) };

                return GetOrdersWithTransactions(group, transByOrderId);
            });

        return results;
    }

    private static IEnumerable<IOrder> GetOrdersWithTransactions(IEnumerable<IOrder> group, Dictionary<string, List<TransactionItem>>? transByOrderId)
    {
        return group.Select(order =>
        {
            if (transByOrderId != null && transByOrderId.TryGetValue(order.Id, out var transactionItems) && order is Order concrete)
                concrete.Transaction =
                    new OrderTransaction(order.Id, transactionItems, order.Currency);
            return order;
        });
    }

    private static Order GetGroupOrderTpSl(IOrder tpOrder, IOrder slOrder, Dictionary<string, List<TransactionItem>>? transByOrderId)
    {
        var sourceOrder = tpOrder.Fills != null && tpOrder.Fills.Any() ? tpOrder : slOrder;

        OrderTransaction? transaction = null;
        if (transByOrderId != null && transByOrderId.TryGetValue(sourceOrder.Id, out var transactionItems))
            transaction = new OrderTransaction(sourceOrder.Id, transactionItems, sourceOrder.Currency);

        return new Order
        {
            Id = tpOrder.Id,
            GroupId = tpOrder.GroupId,
            UserId = tpOrder.UserId,
            AccountId = tpOrder.AccountId,
            Venue = tpOrder.Venue,
            Symbol = tpOrder.Symbol,
            OrderType = OrderType.Tpsl,
            Side = OrderSide.Sell,
            Duration = tpOrder.Duration,
            Quantity = tpOrder.Quantity,
            LimitPrice = tpOrder.LimitPrice,
            StopPrice = slOrder.StopPrice,
            Fills = sourceOrder.Fills,
            Status = tpOrder.Status,
            StatusReason = tpOrder.StatusReason,
            ProviderInfo = tpOrder.ProviderInfo,
            CreatedAt = tpOrder.CreatedAt,
            UpdatedAt = tpOrder.UpdatedAt,
            Channel = tpOrder.Channel,
            Transaction = transaction
        };
    }
}

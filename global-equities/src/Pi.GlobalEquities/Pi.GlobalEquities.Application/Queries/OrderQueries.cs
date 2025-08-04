using Microsoft.Extensions.Logging;
using Pi.Common.CommonModels;
using Pi.GlobalEquities.Application.Commands;
using Pi.GlobalEquities.Application.Exceptions;
using Pi.GlobalEquities.Application.Models.Dto;
using Pi.GlobalEquities.Application.Services.Velexa;
using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Errors;
using Pi.GlobalEquities.Models;

namespace Pi.GlobalEquities.Application.Queries;

public class OrderQueries : IOrderQueries
{
    private readonly IAccountQueries _accountQueries;
    private readonly IVelexaReadService _velexaReadService;
    private readonly ILogger<OrderQueries> _logger;

    private const string OrderTransaction = "TRADE,AUTOCONVERSION,COMMISSION";
    private const string CorporateAction = "STOCK SPLIT,DIVIDEND,US TAX,TAX,CORPORATE ACTION,AUTOCONVERSION";

    public OrderQueries(
        IAccountQueries accountQueries,
        IVelexaReadService velexaReadService,
        ILogger<OrderQueries> logger)
    {
        _accountQueries = accountQueries;
        _velexaReadService = velexaReadService;
        _logger = logger;
    }

    public async Task<IEnumerable<OrderDto>> GetActiveOrders(string userId, string accountId, CancellationToken ct = default)
    {
        var account = await _accountQueries.GetAccountByAccountId(userId, accountId, ct);
        if (account == null)
            throw new GeException(AccountErrors.NotExist);

        var providerAccount = account.GetProviderAccount(Provider.Velexa);
        var activeOrders = await _velexaReadService.GetActiveOrders(providerAccount, ct: ct);

        var mappedOrder = OrderGroupMapper.MapOrdersByGroupIds(activeOrders);
        return mappedOrder.Select(order => new OrderDto(order));
    }

    public async Task<Position?> GetPosition(string account, string symbolId, CancellationToken ct)
    {
        var getAccountSummary = _velexaReadService.GetAccountSummary(account, Currency.USD, ct);
        var getActiveSellOrder = _velexaReadService.GetActiveOrders(account, symbolId, ct);

        await Task.WhenAll(getAccountSummary, getActiveSellOrder);

        var accountSummary = await getAccountSummary;
        var activeSellOrders = await getActiveSellOrder;
        var mappedOrder = OrderGroupMapper.MapOrdersByGroupIds(activeSellOrders);
        var activeSellQuantity = mappedOrder.Sum(order => order.Quantity);

        var position = accountSummary.Positions.FirstOrDefault(x => x.SymbolId == symbolId);
        if (position == null)
            return null;

        var entireQuantity = Convert.ToDecimal(position.EntireQuantity);
        var availableQuantity = Convert.ToDecimal(position.EntireQuantity) - activeSellQuantity;
        var currency = position.Currency;
        var value = Convert.ToDecimal(position.MarketValue);
        var price = Convert.ToDecimal(position.LastPrice);
        var upnl = Convert.ToDecimal(position.Upnl);
        var averagePrice = Convert.ToDecimal(position.AveragePrice);
        var cost = value - upnl;
        var upnlPercentage = cost == 0 ? 0 : 100 * upnl / cost;

        var symbolParts = position.SymbolId?.Split(".");
        if (symbolParts == null || symbolParts.Length != 2)
            throw new ArgumentException($"Wrong Symbol Format {position.SymbolId}");

        return new Position
        {
            Symbol = symbolParts[0],
            Venue = symbolParts[1],
            Currency = currency,
            EntireQuantity = entireQuantity,
            AvailableQuantity = availableQuantity,
            MarketValue = value,
            LastPrice = price,
            Cost = cost,
            AveragePrice = averagePrice,
            Upnl = upnl,
            UpnlPercentage = upnlPercentage,
            SymbolType = position.SymbolType
        };
    }

    public async Task<IEnumerable<OrderDto>> GetOrders(string userId, string accountId, DateTime from, DateTime to,
        OrderSide? side, bool? hasFilled, CancellationToken ct)
    {
        var account = await _accountQueries.GetAccountByAccountId(userId, accountId, ct);
        if (account == null)
            throw new GeException(AccountErrors.NotExist);

        var providerAccount = account.GetProviderAccount(Provider.Velexa);

        var getOrders = _velexaReadService.GetOrders(providerAccount, from, to, ct);
        var getTransactions = _velexaReadService.GetTransactions(providerAccount, from, to, OrderTransaction, ct: ct);

        try
        {
            await Task.WhenAll(getOrders, getTransactions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to Get Orders or/and Transaction. UserId: {UserId}, AccountId: {AccountId}, From: {From}, To: {To}, Side: {Side}, HasFilled: {HasFilled}",
                userId, accountId, from, to, side, hasFilled);
            throw;
        }

        var orders = await getOrders;
        var transactions = await getTransactions;

        var filteredOrder =
            FilterOrders(orders, from, to, side, hasFilled, [OrderStatus.Queued, OrderStatus.Processing]);

        var mappedOrders = OrderGroupMapper.MapOrdersByGroupIds(filteredOrder, transactions);
        return mappedOrders.Select(order => new OrderDto(order));
    }

    private static IEnumerable<IOrder> FilterOrders(
        IList<IOrder> orders,
        DateTime from,
        DateTime to,
        OrderSide? side,
        bool? hasFilled,
        OrderStatus[] excludeStatuses)
    {
        if (!orders.Any())
            return Enumerable.Empty<IOrder>();

        var filteredOrders = orders
            .Where(x => x.ProviderInfo.CreatedAt >= from && x.ProviderInfo.CreatedAt <= to);

        if (excludeStatuses.Length > 0)
        {
            var excludeStatusSet = new HashSet<OrderStatus>(excludeStatuses);
            filteredOrders = filteredOrders.Where(x => !excludeStatusSet.Contains(x.Status));
        }
        if (side is not null)
            filteredOrders = filteredOrders.Where(x => x.Side == side);
        if (hasFilled is not null)
            filteredOrders = filteredOrders.Where(x => hasFilled.Value ? x.Fills.Any() : !x.Fills.Any());

        return filteredOrders;
    }
}


using Microsoft.Extensions.Logging;
using Pi.Client.GlobalMarketData.Api;
using Pi.Client.GlobalMarketData.Model;
using Pi.Client.Sirius.Api;
using Pi.Client.Sirius.Model;
using Pi.Common.Features;
using Pi.GlobalEquities.Services.Configs;
using Pi.GlobalEquities.Services.Wallet;
using Pi.GlobalEquities.Utils;

namespace Pi.GlobalEquities.Services.Velexa;

public class VelexaTradingReadService : ITradingReadService
{

    private readonly IAccountService _accountService;
    private readonly VelexaClient _velexaClient;
    private readonly IWalletService _walletService;
    private IOrderReferenceValidator _orderReferenceValidator;
    private ILogger<VelexaTradingReadService> _logger;

    public VelexaTradingReadService(IAccountService accountService,
        VelexaClient velexaClient,
        IWalletService walletService,
        IOrderReferenceValidator orderReferenceValidator,
        ILogger<VelexaTradingReadService> logger)
    {
        _accountService = accountService;
        _velexaClient = velexaClient;
        _walletService = walletService;
        _orderReferenceValidator = orderReferenceValidator;
        _logger = logger;
    }

    public async Task<IOrder> GetOrder(string orderId, CancellationToken ct)
    {
        var response = await _velexaClient.GetOrder(orderId, ct);
        var order = MapToOrder(response);
        return order;
    }

    public async Task<IEnumerable<Order>> GetOrders(DateTime from, DateTime to, CancellationToken ct)
    {
        var response = await _velexaClient.GetOrders(from, to, ct: ct);
        var orders = response.Select(MapToOrder).Where(x => x != null);
        return orders;
    }

    public async Task<IEnumerable<Order>> GetOrders(string accountId,
        DateTime from, DateTime to,
        OrderSide? side, bool? hasFilled,
        OrderStatus[] excludeStatuses,
        CancellationToken ct)
    {
        var adjustedFrom = from.AddDays(-1);
        var adjustedTo = to.AddDays(1);
        var paginationTo = adjustedTo;
        var totalSortedOrders = new List<Order>();
        bool shouldQueryMore;
        do
        {
            shouldQueryMore = false;
            var response = (await _velexaClient.GetOrders(adjustedFrom, paginationTo, accountId, ct))
                .ToArray();
            var orders = response.Select(MapToOrder).Where(x => x != null).ToArray();

            if (!orders.Any())
                break;

            if (totalSortedOrders.Count > 0)
            {
                var currentTail = totalSortedOrders[^1];
                var duplicateTillIndex = Array.FindIndex(orders, x => x.Id == currentTail.Id);
                if (duplicateTillIndex >= 0)
                {
                    var newItemStartAt = duplicateTillIndex + 1;
                    totalSortedOrders.AddRange(orders[newItemStartAt..]);
                }
                else
                {
                    totalSortedOrders.AddRange(orders);
                }
            }
            else
            {
                totalSortedOrders.AddRange(orders);
            }

            shouldQueryMore = orders.Length == VelexaApiConfig.OrderQueryLimit;
            if (shouldQueryMore)
            {
                paginationTo = orders[^1].ProviderInfo.ModifiedAt;
            }
        } while (shouldQueryMore);

        var results = FilterOrders(totalSortedOrders, from, to, side, hasFilled, excludeStatuses);

        return results;
    }

    private async Task<IEnumerable<IOrder>> GetActiveOrders(string providerAccount, string symbolId = null,
        CancellationToken ct = default)
    {
        var response = await _velexaClient.GetActiveOrders(providerAccount, symbolId, ct);
        var orders = response.Select(MapToOrder).Where(x => x != null);

        return orders;
    }

    public async Task<AccountOverview> GetAccountOverview(IAccount account, Currency currency, CancellationToken ct)
    {
        var providerAccount = account.GetProviderAccount(Provider.Velexa);

        var getPositions = _velexaClient.GetVelexaAccountSummary(providerAccount, Currency.USD, ct);
        var getActiveOrders = GetActiveOrders(providerAccount, ct: ct);
        var getExRate = _walletService.GetExchangeRate(Currency.USD, currency, ct);

        await Task.WhenAll(getPositions, getActiveOrders, getExRate);

        var accSummary = await getPositions;
        var activeOrders = (await getActiveOrders).ToArray();
        var exRate = await getExRate;
        var accBalance =
            await _accountService.GetAccountBalance(account.UserId, account.Id, Currency.USD, accSummary, activeOrders,
                ct);

        var accCurrency = (Currency)Enum.Parse(typeof(Currency), accSummary.currency, true);
        var netAssetValue = Convert.ToDecimal(accSummary.netAssetValue);
        var investment = SummarizeInvestmentPositions(accSummary.positions);

        var hkExRate = activeOrders.Any(x => x.Currency == Currency.HKD)
            ? await _velexaClient.GetExchangeRate(Currency.HKD, Currency.USD, ct)
            : 1;

        var totalActiveCashUsd = activeOrders.Sum(x => x.GetActiveCash(x.Currency == Currency.HKD ? hkExRate : 1));

        var accOverview = new AccountOverview
        (
            accCurrency,
            netAssetValue,
            investment.totalMarketValue,
            investment.totalCost,
            investment.totalUpnl,
            totalActiveCashUsd
        );

        var balance = accBalance.GetBalance(Provider.Velexa, Currency.USD);
        accOverview.AccountId = account.Id;
        accOverview.TradingAccountNoDisplay = account.TradingAccountNo;
        accOverview.TradingAccountNo = account.TradingAccountNo;
        accOverview.TradingLimit = 100000m;
        accOverview.WithdrawableCash = balance;
        accOverview.LineAvailable = balance;

        accOverview.ChangeCurrency(exRate);

        return accOverview;
    }

    public async Task<IList<TransactionItem>> GetTransactions(string accountId, DateTime from, DateTime to,
        CancellationToken ct)
    {
        return await GetAll(from, to, GetPerLimit, returnExtendedInDayDuration: true);

        Task<IEnumerable<VelexaModel.TransactionResponse>> GetPerLimit(DateTime fromDt, DateTime toDt)
            => _velexaClient.GetTransactions(accountId, fromDt, toDt,
                VelexaModel.OperationTypeGroup.OrderTransaction, ct);
    }

    public async Task<IList<TransactionItem>> GetCorporateActionTransactions(string accountId, DateTime from,
        DateTime to,
        CancellationToken ct)
    {
        return await GetAll(from, to, GetPerLimit, returnExtendedInDayDuration: true);

        Task<IEnumerable<VelexaModel.TransactionResponse>> GetPerLimit(DateTime fromDt, DateTime toDt)
            => _velexaClient.GetTransactions(accountId, fromDt, toDt,
                VelexaModel.OperationTypeGroup.CorporateAction, ct);
    }

    private async Task<IList<TransactionItem>> GetAll(DateTime from, DateTime to,
        Func<DateTime, DateTime, Task<IEnumerable<VelexaModel.TransactionResponse>>> getPerLimit,
        bool returnExtendedInDayDuration = false)
    {
        var extendedDayFrom = from.AddDays(-1);
        var extendedDayTo = to.AddDays(1);
        var paginationTo = extendedDayTo;
        var totalDescTrns = new List<TransactionItem>();
        bool shouldQueryMore;
        do
        {
            shouldQueryMore = false;
            var response = await getPerLimit(extendedDayFrom, paginationTo);
            var trns = response.Select(x => x.MapToTransaction()).ToArray();

            if (trns.Length == 0)
                break;

            if (totalDescTrns.Count > 0)
            {
                var currentTail = totalDescTrns[^1];
                var duplicateTillIndex = Array.FindIndex(trns, x => x.Id == currentTail.Id);
                if (duplicateTillIndex >= 0)
                {
                    var newItemStartAt = duplicateTillIndex + 1;
                    totalDescTrns.AddRange(trns[newItemStartAt..]);
                }
                else
                {
                    totalDescTrns.AddRange(trns);
                }
            }
            else
            {
                totalDescTrns.AddRange(trns);
            }

            shouldQueryMore = trns.Length == VelexaApiConfig.TransactionQueryLimit;
            if (shouldQueryMore)
            {
                paginationTo = DateTimeUtils.ConvertToDateTimeUtc(trns[^1].Timestamp);
            }
        } while (shouldQueryMore);

        if (returnExtendedInDayDuration)
            return totalDescTrns;

        totalDescTrns = Filter(totalDescTrns, from, to);

        return totalDescTrns;
    }

    private List<TransactionItem> Filter(List<TransactionItem> totalDescTrns, DateTime from, DateTime to)
    {
        var fromStamp = DateTimeUtils.ConvertToTimestamp(from);
        var toStamp = DateTimeUtils.ConvertToTimestamp(to);
        var endIndex = totalDescTrns.FindLastIndex(x => x.Timestamp >= fromStamp);
        var startIndex = totalDescTrns.FindLastIndex(x => x.Timestamp > toStamp) + 1;

        if (endIndex < 0)
            return new List<TransactionItem>();

        var itemCount = endIndex - startIndex + 1;
        totalDescTrns = totalDescTrns.GetRange(startIndex, itemCount);

        return totalDescTrns;
    }

    protected Order MapToOrder(VelexaModel.OrderResponse orderResponse)
    {
        try
        {
            var clientTag = orderResponse.clientTag;
            var orderTagInfo = _orderReferenceValidator.Extract(clientTag, orderResponse.accountId);

            return orderResponse.MapToOrder(orderTagInfo);
        }
        catch (InvalidDataException ex)
        {
            _logger.LogError(ex,
                "ClientTagValidationFailed The error occurs when trying to validate and extract clientTag. OrderId: {OrderId}, VelexaAccount: {VelexaAccount}, ClientTag:{ClientTag}",
                orderResponse.orderId, orderResponse.accountId, orderResponse.clientTag);
            return null;
        }
    }

    private (decimal totalCost,
        decimal totalUpnl,
        decimal totalMarketValue,
        IEnumerable<Position> positions)
        SummarizeInvestmentPositions(IEnumerable<VelexaModel.VelexaPosition> vPositions,
            IEnumerable<IOrder> activeOrders = null, IList<TickerResponse> marketDataTickers = null)
    {
        var totalCost = 0m;
        var totalUpnl = 0m;
        var totalMarketValue = 0m;
        var positions = new List<Position>();
        var activeOrdersBySymbolId = activeOrders?
                                         .GroupBy(x => x.SymbolId)
                                         .ToDictionary(x => x.Key, y => y.ToList())
                                     ?? new();

        foreach (var vpos in vPositions)
        {
            var quantity = Convert.ToDecimal(vpos.quantity);
            if (quantity <= 0 && vpos.symbolType != "STOCK")
                continue;

            decimal activeSellQuantity = 0m;
            if (activeOrdersBySymbolId.TryGetValue(vpos.symbolId, out var activeSymbolOrders))
            {
                activeSellQuantity = activeSymbolOrders
                    .Where(x => x.Side == OrderSide.Sell)
                    .Sum(x => x.Quantity);
            }

            decimal? marketPrice = null;
            if (marketDataTickers != null)
            {
                string priceString = marketDataTickers
                    .FirstOrDefault(t => $"{t.Symbol}.{t.Venue}" == vpos.symbolId)?.Price;

                if (!string.IsNullOrWhiteSpace(priceString) && decimal.TryParse(priceString, out decimal parsedPrice))
                {
                    marketPrice = parsedPrice;
                }
                else
                {
                    _logger.LogWarning("Invalid or empty Price: '{Price}' for symbolId: '{SymbolId}'", priceString, vpos.symbolId);
                }
            }

            var pos = vpos.MapToPosition(activeSellQuantity, marketPrice);
            positions.Add(pos);

            totalCost += pos.ConvertedCost;
            totalUpnl += pos.ConvertedUpnl;
            totalMarketValue += pos.ConvertedMarketValue;
        }

        return (totalCost, totalUpnl, totalMarketValue, positions);
    }

    private static IEnumerable<Order> FilterOrders(
        IEnumerable<Order> orders,
        DateTime from,
        DateTime to,
        OrderSide? side,
        bool? hasFilled,
        OrderStatus[] excludeStatuses)
    {
        if (!orders.Any())
            return Enumerable.Empty<Order>();

        var filteredOrders = orders
            .Where(x => x.ProviderInfo.CreatedAt >= from && x.ProviderInfo.CreatedAt <= to);

        if (excludeStatuses.Length > 0)
            filteredOrders = filteredOrders.Where(x => !excludeStatuses.Contains(x.Status));
        if (side is not null)
            filteredOrders = filteredOrders.Where(x => x.Side == side);
        if (hasFilled is not null)
            filteredOrders = filteredOrders.Where(x => x.Fills.Any());

        return filteredOrders;
    }

    public class TickerResponse
    {
        public string Symbol { get; init; }
        public string Venue { get; init; }
        public string Price { get; init; }
    }
}



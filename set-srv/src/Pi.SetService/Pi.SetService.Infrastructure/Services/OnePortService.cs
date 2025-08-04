using Pi.Client.BondApi.Api;
using Pi.Client.OnePort.GW.DB2.Api;
using Pi.Client.OnePort.GW.DB2.Client;
using Pi.Client.OnePort.GW.DB2.Model;
using Pi.Client.OnePort.GW.TCP.Model;
using Pi.Common.Features;
using Pi.SetService.Application.Models;
using Pi.SetService.Application.Services.OneportService;
using Pi.SetService.Application.Utils;
using Pi.SetService.Domain.AggregatesModel.AccountAggregate;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.SetService.Infrastructure.Factories;
using Pi.SetService.Infrastructure.Utils;
using IDb2TradingApi = Pi.Client.OnePort.GW.DB2.Api.ITradingApi;
using ITcpTradingApi = Pi.Client.OnePort.GW.TCP.Api.ITradingApi;

namespace Pi.SetService.Infrastructure.Services;

public class OnePortService(
    IDb2TradingApi db2TradingApi,
    ITcpTradingApi tcpTradingApi,
    IAccountApi accountApi,
    IMarketDataApi bondMarketDataApi,
    IFeatureService featureService)
    : IOnePortService
{
    public async Task<List<OnlineOrder>> GetOrdersByAccountNo(string tradingAccountNo,
        CancellationToken cancellationToken = default)
    {
        var response =
            await db2TradingApi.GetOrdersAsync(OnePortFactory.NewAccountNo(tradingAccountNo), 0, cancellationToken);
        var result = response.Data.Select(EntityFactory.NewSetOnlineOrder).ToList();

        return result;
    }

    public async Task<OnlineOrder?> GetOrderByAccountNoAndOrderNo(string tradingAccountNo, string orderNo,
        CancellationToken cancellationToken = default)
    {
        var orders = await GetOrdersByAccountNo(tradingAccountNo, cancellationToken);
        return orders.Find(order => orderNo == order.OrderNo.ToString() && tradingAccountNo == order.TradingAccountNo);
    }

    public async Task<List<OfflineOrder>> GetOfflineOrdersByAccountNo(string tradingAccountNo,
        CancellationToken cancellationToken = default)
    {
        var response =
            await db2TradingApi.GetOfflineOrdersAsync(OnePortFactory.NewAccountNo(tradingAccountNo), 0,
                cancellationToken);
        var result = response.Data
            .Select(EntityFactory.NewSetOfflineOrder)
            .ToList();

        return result;
    }

    public async Task<OfflineOrder?> GetOfflineOrderByAccountNoAndOrderNo(string tradingAccountNo, string orderNo,
        CancellationToken cancellationToken = default)
    {
        var orders = await GetOfflineOrdersByAccountNo(tradingAccountNo, cancellationToken);
        return orders.Find(order => orderNo == order.OrderNo.ToString() && tradingAccountNo == order.TradingAccountNo);
    }

    public async Task<List<Deal>> GetDealsByAccountNo(string accountNo, CancellationToken cancellationToken = default)
    {
        var response = await db2TradingApi.GetAccountDealsAsync(accountNo, 0, cancellationToken);
        var result = response.Data
            .Select(EntityFactory.NewDeal)
            .ToList();

        return result;
    }

    public async Task<BrokerOrderResponse> CreateNewOrder(NewOrder requestedOrder)
    {
        var payload = OnePortFactory.NewNewOrder7A(requestedOrder);
        var response = await tcpTradingApi.NewOrderAsync(payload);

        return ApplicationFactory.NewBrokerOrderResponse(response);
    }

    public async Task<NewOfflineOrderResponse> CreateNewOfflineOrder(NewOfflineOrder requestedOrder)
    {
        var payload = OnePortFactory.NewPiOnePortDb2ModelsOfflineOrderRequest(requestedOrder);
        await db2TradingApi.NewOfflineOrderAsync(payload);

        return new NewOfflineOrderResponse(requestedOrder.OrderNo);
    }

    public async Task<BrokerOrderResponse> CancelOrder(CancelOrder requestedOrder,
        CancellationToken cancellationToken = default)
    {
        var response = await tcpTradingApi.CancelOrderAsync(
            new PiOnePortTCPModelsPacketsDataTransferDataTransferOrderCancelRequest7C(
                requestedOrder.OrderNo,
                requestedOrder.BrokerOrderId,
                requestedOrder.EnterId
            ), cancellationToken);

        return ApplicationFactory.NewBrokerOrderResponse(response);
    }

    public async Task CancelOfflineOrder(CancelOfflineOrder requestedOrder,
        CancellationToken cancellationToken = default)
    {
        await db2TradingApi.CancelOfflineOrderAsync(new PiOnePortDb2ModelsCancelOfflineOrderRequest(
            DateTimeHelper.ConvertThTimeFromUtc(requestedOrder.OrderDateTime),
            requestedOrder.BrokerOrderId,
            requestedOrder.CancelId,
            requestedOrder.CancelDateTime,
            requestedOrder.DelFlag
        ), cancellationToken);
    }

    public async Task<BrokerOrderResponse> ChangeOrder(ChangeOrder requestedOrder,
        CancellationToken cancellationToken = default)
    {
        var response = await tcpTradingApi.ChangeOrderAsync(OnePortFactory.NewChangeOrderRequest7M(requestedOrder),
            cancellationToken);

        return ApplicationFactory.NewBrokerOrderResponse(response);
    }

    public async Task ChangeOfflineOrder(ChangeOfflineOrder requestedOrder,
        CancellationToken cancellationToken = default)
    {
        await db2TradingApi.UpdateOfflineOrderAsync(
            OnePortFactory.NewPiOnePortDb2ModelsOfflineOrderRequest(requestedOrder), cancellationToken);
    }

    public async Task<List<AvailableCashBalance>> GetAvailableCashBalances(string tradingAccountNo,
        CancellationToken cancellationToken = default)
    {
        var response =
            await accountApi.GetAccountsAvailableAsync(OnePortFactory.NewAccountNo(tradingAccountNo), 0,
                cancellationToken);
        var accountBalanceAvailable = response.Data
            .Select(EntityFactory.NewAccountBalanceAvailable)
            .ToList();
        return accountBalanceAvailable;
    }

    public async Task<List<AvailableCreditBalance>> GetAvailableCreditBalances(string tradingAccountNo,
        CancellationToken cancellationToken = default)
    {
        var response = await accountApi.GetCreditBalanceAccountsAvailableAsync(
            OnePortFactory.NewAccountNo(tradingAccountNo),
            0,
            cancellationToken);
        var result = response.Data.Select(EntityFactory.NewAccountAvailableCreditBalance).ToList();

        return result;
    }

    public async Task<List<AccountPosition>> GetPositions(string tradingAccountNo,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // NOTE: passing page as null because Sirius code don't support pagination they always send page as 0 on the query yet
            var accountPositionsTask =
                accountApi.GetAccountPositionAsync(OnePortFactory.NewAccountNo(tradingAccountNo), 0, cancellationToken);
            var bondSymbolsTask = GetBondSymbols(cancellationToken);
            await Task.WhenAll(accountPositionsTask, bondSymbolsTask);
            var accountPositionsResponse = await accountPositionsTask;
            var bondSymbols = await bondSymbolsTask;

            var accountPositions = accountPositionsResponse.Data
                .Where(q => !bondSymbols.Contains(q.SecSymbol))
                .Select(EntityFactory.NewAccountPosition)
                .ToList();

            return accountPositions;
        }
        catch (ApiException)
        {
            return new List<AccountPosition>();
        }
    }

    public async Task<List<AccountPositionCreditBalance>> GetPositionsCreditBalance(string tradingAccountNo,
        CancellationToken cancellationToken = default)
    {
        var accountPositionsTask = accountApi.GetAccountCreditBalancePositionAsync(
            OnePortFactory.NewAccountNo(tradingAccountNo),
            0,
            cancellationToken);
        var bondSymbolsTask = GetBondSymbols(cancellationToken);
        await Task.WhenAll(accountPositionsTask, bondSymbolsTask);
        var accountPositionsResponse = await accountPositionsTask;
        var bondSymbols = await bondSymbolsTask;

        return accountPositionsResponse.Data
            .Where(q => !bondSymbols.Contains(q.SecSymbol))
            .Select(EntityFactory.NewAccountPosition)
            .ToList();
    }

    private async Task<IEnumerable<string>> GetBondSymbols(CancellationToken cancellationToken = default)
    {
        if (featureService.IsOff(Features.ExcludeBond)) return [];

        try
        {
            var response = await bondMarketDataApi.InternalBondsSymbolsGetAsync(cancellationToken);

            return response.Data.Symbols;
        }
        catch (ApiException)
        {
            return await Task.FromResult<IEnumerable<string>>(BondSymbols.Symbols);
        }
    }
}

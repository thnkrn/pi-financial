using Pi.SetService.Application.Exceptions;
using Pi.SetService.Application.Models;
using Pi.SetService.Application.Services.OneportService;
using Pi.SetService.Domain.AggregatesModel.AccountAggregate;
using Pi.SetService.Domain.AggregatesModel.ErrorAggregate;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.SetService.Infrastructure.Factories;

namespace Pi.SetService.Infrastructure.Services;

public class MaintenanceOnePortService(OnePortService onePortService) : IOnePortService
{
    public Task<List<OnlineOrder>> GetOrdersByAccountNo(string tradingAccountNo, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new List<OnlineOrder>());
    }

    public Task<OnlineOrder?> GetOrderByAccountNoAndOrderNo(string tradingAccountNo, string orderNo,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<OnlineOrder?>(null);
    }

    public Task<List<OfflineOrder>> GetOfflineOrdersByAccountNo(string tradingAccountNo, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new List<OfflineOrder>());
    }

    public Task<OfflineOrder?> GetOfflineOrderByAccountNoAndOrderNo(string tradingAccountNo, string orderNo,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<OfflineOrder?>(null);
    }

    public Task<List<Deal>> GetDealsByAccountNo(string accountNo, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new List<Deal>());
    }

    public async Task<BrokerOrderResponse> CreateNewOrder(NewOrder requestedOrder)
    {
        return await onePortService.CreateNewOrder(requestedOrder);
    }

    public Task<NewOfflineOrderResponse> CreateNewOfflineOrder(NewOfflineOrder requestedOrder)
    {
        throw new SetException(SetErrorCode.SE209);
    }

    public async Task<BrokerOrderResponse> CancelOrder(CancelOrder requestedOrder, CancellationToken cancellationToken = default)
    {
        return await onePortService.CancelOrder(requestedOrder, cancellationToken);
    }

    public Task CancelOfflineOrder(CancelOfflineOrder requestedOrder, CancellationToken cancellationToken = default)
    {
        throw new SetException(SetErrorCode.SE209);
    }

    public async Task<BrokerOrderResponse> ChangeOrder(ChangeOrder requestedOrder, CancellationToken cancellationToken = default)
    {
        return await onePortService.ChangeOrder(requestedOrder, cancellationToken);
    }

    public Task ChangeOfflineOrder(ChangeOfflineOrder requestedOrder, CancellationToken cancellationToken = default)
    {
        throw new SetException(SetErrorCode.SE209);
    }

    public Task<List<AccountPosition>> GetPositions(string tradingAccountNo, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new List<AccountPosition>());
    }

    public Task<List<AccountPositionCreditBalance>> GetPositionsCreditBalance(string tradingAccountNo, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new List<AccountPositionCreditBalance>());
    }

    public Task<List<AvailableCashBalance>> GetAvailableCashBalances(string tradingAccountNo, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new List<AvailableCashBalance>()
        {
            new()
            {
                Ar = 0,
                Ap = 0,
                ArTrade = 0,
                ApTrade = 0,
                TotalBuyMatch = 0,
                TotalBuyUnmatch = 0,
                TradingAccountNo = tradingAccountNo,
                AccountNo = OnePortFactory.NewAccountNo(tradingAccountNo),
                TraderId = "",
                CreditLimit = 0,
                BuyCredit = 0,
                CashBalance = 0
            }
        });
    }

    public Task<List<AvailableCreditBalance>> GetAvailableCreditBalances(string tradingAccountNo, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new List<AvailableCreditBalance>()
        {
            new()
            {
                Liability = 0,
                Asset = 0,
                Equity = 0,
                MarginRequired = 0,
                ExcessEquity = 0,
                CallForce = 0,
                CallMargin = 0,
                TradingAccountNo = tradingAccountNo,
                AccountNo = OnePortFactory.NewAccountNo(tradingAccountNo),
                TraderId = "",
                CreditLimit = 0,
                BuyCredit = 0,
                CashBalance = 0
            }
        });
    }
}

using Pi.BackofficeService.Application.Services.WalletService;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;
using Pi.BackofficeService.Infrastructure.Factories;
using Pi.Client.WalletService.Api;
using Pi.Client.WalletService.Client;
using Pi.Client.WalletService.Model;

namespace Pi.BackofficeService.Infrastructure.Services;

public class DepositWithdrawService : IDepositWithdrawService
{
    private readonly ITransactionApi _transactionApi;

    public DepositWithdrawService(ITransactionApi transactionApi)
    {
        _transactionApi = transactionApi;
    }

    public async Task<TransactionV2?> GetTransactionV2ByTransactionNo(string transactionNo, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _transactionApi.InternalTransactionTransactionNoGetAsync(transactionNo, "Initial", cancellationToken);
            return EntityFactory.NewTransactionV2(response.Data);
        }
        catch (ApiException)
        {
            return null;
        }
    }

    public async Task<PaginateResponse<TransactionHistoryV2>> GetTransactionHistoriesV2(TransactionHistoryV2Filter filters, CancellationToken cancellationToken = default)
    {
        var channel =
            Enum.TryParse(filters.Channel.ToString(), true, out Channel ch)
                ? ClientFactory.NewChannelV2(ch)
                : (PiWalletServiceIntegrationEventsAggregatesModelChannel?)null;
        var productList = new List<PiWalletServiceIntegrationEventsAggregatesModelProduct>();
        if (filters.Product != null)
        {
            productList.AddRange(filters.Product.Select(EntityFactory.MapProductToWalletModelProduct));
        }
        else
        {
            productList = null;
        }

        var response = await _transactionApi.InternalTransactionsGetAsync(
            filters.TransactionNo,
            productList,
            (PiWalletServiceDomainAggregatesModelTransactionAggregateStatus?)filters.Status,
            filters.State,
            (PiWalletServiceIntegrationEventsAggregatesModelTransactionType?)filters.TransactionType,
            channel,
            filters.CreatedAtFrom,
            filters.CreatedAtTo,
            filters.EffectiveDateFrom,
            filters.EffectiveDateTo,
            filters.PaymentReceivedFrom,
            filters.PaymentReceivedTo,
            filters.CustomerCode,
            filters.AccountCode,
            filters.BankAccountNo,
            filters.BankCode,
            filters.BankName,
            null,
            filters.AccountId,
            filters.Page,
            filters.PageSize,
            filters.OrderBy,
            filters.OrderDir,
            cancellationToken
        );

        var transactionHistory = response.Data.Select(EntityFactory.NewTransactionHistoryV2).ToList();

        return new PaginateResponse<TransactionHistoryV2>(transactionHistory, response.Page, response.PageSize, response.Total, response.OrderBy, response.OrderDir);
    }

    public Task<List<DepositChannel>> GetDepositChannels()
    {
        var depositChannels = new List<DepositChannel>();
        Enum.GetValues(typeof(Channel)).Cast<PiWalletServiceIntegrationEventsAggregatesModelChannel>()
            .ToList()
            .ForEach(q =>
            {
                var converted = EntityFactory.NewDepositChannel(q);
                if (converted == null) return;

                depositChannels.Add((DepositChannel)converted);
            });

        return Task.FromResult(depositChannels);
    }

    public Task<List<WithdrawChannel>> GetWithdrawChannels()
    {
        var withdrawChannels = new List<WithdrawChannel>();
        Enum.GetValues(typeof(Channel)).Cast<PiWalletServiceIntegrationEventsAggregatesModelChannel>()
            .ToList()
            .ForEach(q =>
            {
                var converted = EntityFactory.NewWithdrawChannel(q);
                if (converted == null) return;

                withdrawChannels.Add((WithdrawChannel)converted);
            });

        return Task.FromResult(withdrawChannels);
    }

    public Task<List<Product>> GetProducts()
    {
        var products = new List<Product>();
        Enum.GetValues(typeof(PiWalletServiceIntegrationEventsAggregatesModelProduct))
            .Cast<PiWalletServiceIntegrationEventsAggregatesModelProduct>()
            .ToList()
            .ForEach(q =>
            {
                var account = EntityFactory.NewProduct(q);
                if (account == null) return;

                products.Add((Product)account);
            });

        return Task.FromResult(products);
    }
}

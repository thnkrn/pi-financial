using Pi.BackofficeService.Application.Services.WalletService;
using Pi.BackofficeService.Domain.AggregateModels.TransferCashAggregate;
using Pi.BackofficeService.Infrastructure.Factories;
using Pi.Client.WalletService.Api;
using Pi.Client.WalletService.Client;
using Pi.Client.WalletService.Model;

namespace Pi.BackofficeService.Infrastructure.Services;

public class TransferCashService(ITransferApi transferApi) : ITransferCashService
{
    public async Task<PaginateResponse<TransferCash>?> GetTransferCashHistory(TransferCashTransactionFilter filters,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await transferApi.InternalTransferCashGetAsync(
                null,
                null,
                null,
                null,
                filters.TransactionNo,
                (PiWalletServiceDomainAggregatesModelTransactionAggregateStatus?)filters.Status,
                filters.State,
                filters.CreatedAtFrom,
                filters.CreatedAtTo,
                filters.OtpConfirmedDateFrom,
                filters.OtpConfirmedDateTo,
                filters.TransferFromAccountCode,
                filters.TransferToAccountCode,
                null,
                filters.TransferFromExchangeMarket != null
                    ? ClientFactory.NewProduct(filters.TransferFromExchangeMarket)
                    : null,
                filters.TransferToExchangeMarket != null
                    ? ClientFactory.NewProduct(filters.TransferToExchangeMarket)
                    : null,
                null,
                null,
                filters.Page,
                filters.PageSize,
                filters.OrderBy,
                filters.OrderDir,
                cancellationToken);

            var transferCash = response.Data.Select(EntityFactory.NewTransferCash).ToList();

            return new PaginateResponse<TransferCash>(transferCash!, response.Page, response.PageSize, response.Total,
                response.OrderBy, response.OrderDir)!;
        }
        catch (ApiException)
        {
            return null;
        }
    }

    public async Task<TransferCash?> GetTransferCashByTransactionNo(string transactionNo,
        CancellationToken cancellationToken)
    {
        try
        {
            var response =
                await transferApi.InternalTransferCashTransactionNoGetAsync(transactionNo, cancellationToken);
            return EntityFactory.NewTransferCash(response.Data);
        }
        catch (ApiException)
        {
            return null;
        }
    }
}
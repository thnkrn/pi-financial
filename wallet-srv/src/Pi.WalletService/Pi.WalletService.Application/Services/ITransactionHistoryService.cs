using Pi.WalletService.Application.Models;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Services;

public record SiriusAuthentication(string Sid, string DeviceId, string? Device);

public interface ITransactionHistoryService
{
    Task<List<Transaction>> GetTransactionHistory(
        SiriusAuthentication authentication,
        string accountId,
        Product product,
        TransactionType transactionType,
        PaginateRequest paginateRequest,
        DateOnly beginDate,
        DateOnly endDate);
}
using Pi.WalletService.Application.Models;
namespace Pi.WalletService.API.Models;

public record TransactionReport(
    List<Transaction> Transactions,
    TransactionSummary TransactionSummary);
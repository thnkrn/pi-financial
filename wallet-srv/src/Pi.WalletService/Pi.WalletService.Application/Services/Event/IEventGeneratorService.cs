using Pi.WalletService.Application.Commands.Deposit;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Domain.Events.Deposit;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Services.Event;

/// <summary>
/// For mocking test case on UAT only.
/// </summary>
public interface IEventGeneratorService
{
    Task<KkpDeposit> GenerateKkpDepositEvent(string transactionId, MockQrDepositEventState eventName, CancellationToken cancellationToke);
    Task<KkpDeposit> GenerateKkpDepositEventV2(string transactionId, MockQrDepositEventStateV2 eventName, CancellationToken cancellationToken);

    Task<bool> MockDepositWithdrawTransaction(string transactionNo, TransactionType transactionType, Product product,
        MockDepositWithdrawReasons mockDepositWithdrawReason, CancellationToken cancellationToken);
    Task<bool> MockDepositWithdrawTransactionV2(string transactionNo, TransactionType transactionType, Product product,
        MockDepositWithdrawReasons mockDepositWithdrawReason, CancellationToken cancellationToken);

    Task<bool> MockGlobalTransaction(string transactionNo, MockGlobalTransactionReasons mockDepositWithdrawReason,
        CancellationToken cancellationToken);
    Task<bool> MockGlobalTransactionV2(string transactionNo, TransactionType transactionType, MockGlobalTransactionReasons mockDepositWithdrawReason,
        CancellationToken cancellationToken);
}

using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
namespace Pi.WalletService.Domain.Events.WithdrawEntrypoint;

public record WithdrawEntrypointRequest(
    Guid CorrelationId,
    string UserId,
    string CustomerCode,
    string AccountCode,
    Product Product,
    Channel Channel,
    Purpose Purpose,
    decimal RequestedAmount,
    string? BankAccountNo,
    string? BankAccountName,
    string? BankAccountTaxId,
    string? BankName,
    string? BankCode,
    string? BankBranchCode,
    string? CustomerName,
    long? CustomerId,
    string? GlobalAccount,
    Currency? RequestedCurrency,
    string? FxTransactionId,
    decimal? FxMarkUp,
    Guid DeviceId,
    Guid? RequestId,
    string? ResponseAddress,
    DateTime EffectiveDate
);

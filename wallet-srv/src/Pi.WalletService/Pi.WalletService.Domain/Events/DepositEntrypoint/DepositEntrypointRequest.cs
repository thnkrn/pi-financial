using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
namespace Pi.WalletService.Domain.Events.DepositEntrypoint;

public record DepositEntrypointRequest(
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
    decimal? RequestedFxAmount,
    Currency? RequestedCurrency,
    Currency? RequestedFxCurrency,
    decimal? FxMarkUp,
    Guid DeviceId,
    Guid? RequestId,
    string? ResponseAddress,
    DateTime EffectiveDate,
    int? QrCodeExpiredTimeInMinute = 60
);

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.AggregatesModel.WalletAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Infrastructure.EntityConfigs;

public class ActivityLogMap : IEntityTypeConfiguration<ActivityLog>
{
    public ActivityLogMap()
    {
    }

    public void Configure(EntityTypeBuilder<ActivityLog> entity)
    {
        entity.Property(s => s.CorrelationId).HasMaxLength(36).IsRequired();
        entity.Property(s => s.TransactionNo).HasMaxLength(36);
        entity.Property(s => s.TransactionType).HasConversion(new EnumToStringConverter<TransactionType>());
        entity.Property(s => s.UserId).HasMaxLength(36);
        entity.Property(s => s.AccountCode).HasMaxLength(36);
        entity.Property(s => s.CustomerCode).HasMaxLength(36);
        entity.Property(s => s.Channel).HasConversion(new EnumToStringConverter<Channel>()).IsRequired();
        entity.Property(s => s.Product).HasConversion(new EnumToStringConverter<Product>()).IsRequired();
        entity.Property(s => s.Purpose).HasConversion(new EnumToStringConverter<Purpose>()).IsRequired();
        entity.Property(s => s.StateMachine).HasMaxLength(100).IsRequired();
        entity.Property(s => s.State).HasMaxLength(100).IsRequired();
        entity.Property(s => s.PaymentReceivedDateTime).HasMaxLength(6);
        entity.Property(s => s.PaymentReceivedAmount).HasPrecision(65, 30);
        entity.Property(s => s.PaymentDisbursedDateTime).HasMaxLength(6);
        entity.Property(s => s.PaymentDisbursedAmount).HasPrecision(65, 30);
        entity.Property(s => s.PaymentConfirmedAmount).HasPrecision(65, 30);
        entity.Property(s => s.OtpRequestRef).HasMaxLength(36);
        entity.Property(s => s.OtpRequestId).HasMaxLength(36);
        entity.Property(s => s.OtpConfirmedDateTime).HasMaxLength(6);
        entity.Property(s => s.RequestedAmount).HasPrecision(65, 30).IsRequired();
        entity.Property(s => s.Fee).HasPrecision(65, 30);
        entity.Property(s => s.CreatedAt).HasMaxLength(6);
        entity.Property(s => s.CustomerName).IsEncrypted().HasMaxLength(500);
        entity.Property(s => s.BankAccountName).IsEncrypted().HasMaxLength(500);
        entity.Property(s => s.BankName).HasMaxLength(50);
        entity.Property(s => s.BankCode).HasMaxLength(10);
        entity.Property(s => s.BankAccountNo).HasMaxLength(10);
        entity.Property(s => s.BankBranchCode).HasMaxLength(10);
        entity.Property(s => s.BankAccountTaxId).IsEncrypted().HasMaxLength(100);
        entity.Property(s => s.FailedReason).HasMaxLength(200);
        entity.Property(s => s.DepositGeneratedDateTime).HasMaxLength(6);
        entity.Property(s => s.QrTransactionNo).HasMaxLength(36);
        entity.Property(s => s.QrTransactionRef).HasMaxLength(36);
        entity.Property(s => s.QrValue).HasMaxLength(36);
        entity.Property(s => s.RequestedCurrency).HasConversion(new EnumToStringConverter<Currency>());
        entity.Property(s => s.RequestedAmountWithCurrency).HasMaxLength(36);
        entity.Property(s => s.RequestedFxAmount).HasPrecision(65, 30);
        entity.Property(s => s.TransferFee).HasPrecision(65, 30);
        entity.Property(s => s.PaymentReceivedCurrency).HasConversion(new EnumToStringConverter<Currency>());
        entity.Property(s => s.RequestedFxCurrency).HasConversion(new EnumToStringConverter<Currency>());
        entity.Property(s => s.RequestedFxAmountWithCurrency).HasMaxLength(36);
        entity.Property(s => s.FxTransactionId).HasMaxLength(36);
        entity.Property(s => s.FxInitiateRequestDateTime).HasMaxLength(6);
        entity.Property(s => s.FxConfirmedExchangeRate).HasPrecision(65, 30);
        entity.Property(s => s.FxConfirmedAmount).HasPrecision(65, 30);
        entity.Property(s => s.FxConfirmedCurrency).HasConversion(new EnumToStringConverter<Currency>());
        entity.Property(s => s.FxConfirmedDateTime).HasMaxLength(6);
        entity.Property(s => s.FxConfirmedAmountWithCurrency).HasMaxLength(36);
        entity.Property(s => s.TransferFromAccount).HasMaxLength(36);
        entity.Property(s => s.TransferAmount).HasPrecision(65, 30);
        entity.Property(s => s.TransferToAccount).HasMaxLength(36);
        entity.Property(s => s.TransferCurrency).HasConversion(new EnumToStringConverter<Currency>());
        entity.Property(s => s.TransferAmountWithCurrency).HasMaxLength(36);
        entity.Property(s => s.TransferRequestTime).HasMaxLength(6);
        entity.Property(s => s.TransferCompleteTime).HasMaxLength(6);
        entity.Property(s => s.FailedReason).HasMaxLength(200);
        entity.Property(s => s.RequestId).HasMaxLength(36);
        entity.Property(s => s.RequesterDeviceId).HasMaxLength(36);

        entity.HasIndex(s => s.CorrelationId);
    }
}

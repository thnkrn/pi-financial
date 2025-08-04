using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using QrDepositState = Pi.WalletService.Domain.AggregatesModel.QrDepositAggregate.QrDepositState;

namespace Pi.WalletService.Infrastructure.EntityConfigs;

public class QrDepositStateMap : SagaClassMap<QrDepositState>
{
    public QrDepositStateMap()
    {
    }

    protected override void Configure(EntityTypeBuilder<QrDepositState> entity, ModelBuilder builder)
    {
        // Properties
        entity.Property(s => s.CorrelationId).HasMaxLength(36).IsRequired();
        entity.Property(s => s.CurrentState).HasMaxLength(100);
        entity.Property(s => s.TransactionNo).HasMaxLength(36);
        entity.Property(s => s.Product).HasConversion(new EnumToStringConverter<Product>());
        entity.Property(s => s.Channel).HasConversion(new EnumToStringConverter<Channel>());
        entity.Property(s => s.QrTransactionNo);
        entity.Property(s => s.QrTransactionRef);
        entity.Property(s => s.QrValue);
        entity.Property(s => s.QrCodeExpiredTimeInMinute);
        entity.Property(s => s.DepositQrGenerateDateTime).HasMaxLength(6);
        entity.Property(s => s.PaymentReceivedDateTime).HasMaxLength(6);
        entity.Property(s => s.PaymentReceivedAmount).HasPrecision(65, 30);
        entity.Property(s => s.Fee).HasPrecision(65, 30);
        entity.Property(s => s.FailedReason);
        entity.Property(s => s.RowVersion).IsRowVersion();
        entity.Property(s => s.CreatedAt).HasMaxLength(6).HasDefaultValue(DateTime.Now);
        entity.Property(s => s.UpdatedAt).HasMaxLength(6);

        entity.HasIndex(s => s.CorrelationId).IsUnique();
    }

}

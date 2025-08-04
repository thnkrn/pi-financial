using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.RecoveryAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Infrastructure.EntityConfigs;

public class RecoveryStateMap : SagaClassMap<RecoveryState>
{
    protected override void Configure(EntityTypeBuilder<RecoveryState> entity, ModelBuilder model)
    {
        var currencyEnumToStringConverter = new EnumToStringConverter<Currency>();
        entity.Property(s => s.CorrelationId).HasMaxLength(36).IsRequired();
        entity.Property(s => s.CurrentState).HasMaxLength(100);
        entity.Property(s => s.Product).HasConversion(new EnumToStringConverter<Product>());
        entity.Property(s => s.TransactionType).HasConversion(new EnumToStringConverter<TransactionType>());
        entity.Property(s => s.GlobalAccount);
        entity.Property(s => s.TransferFromAccount);
        entity.Property(s => s.TransferAmount).HasPrecision(65, 30);
        entity.Property(s => s.TransferToAccount);
        entity.Property(s => s.TransferCurrency).HasConversion(currencyEnumToStringConverter);
        entity.Property(s => s.TransferRequestTime).HasMaxLength(6);
        entity.Property(s => s.TransferCompleteTime).HasMaxLength(6);
        entity.Property(s => s.FailedReason);
        entity.Property(s => s.RequestId);
        entity.Property(s => s.RowVersion).IsRowVersion().IsRequired();
        entity.Property(s => s.CreatedAt).HasMaxLength(6);
        entity.Property(s => s.UpdatedAt).HasMaxLength(6);

        entity.HasIndex(s => s.CorrelationId).IsUnique();
    }
}

using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.AggregatesModel.UpBackAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Infrastructure.EntityConfigs;

public class UpBackStateMap : SagaClassMap<UpBackState>
{
    public UpBackStateMap()
    {
    }

    protected override void Configure(EntityTypeBuilder<UpBackState> entity, ModelBuilder builder)
    {
        // Properties
        entity.Property(s => s.CorrelationId).HasMaxLength(36).IsRequired();
        entity.Property(s => s.CurrentState).HasMaxLength(100);
        entity.Property(s => s.Product).HasConversion(new EnumToStringConverter<Product>());
        entity.Property(s => s.Channel).HasConversion(new EnumToStringConverter<Channel>());
        entity.Property(s => s.TransactionType).HasConversion(new EnumToStringConverter<TransactionType>()).IsRequired();
        entity.Property(s => s.FailedReason);
        entity.Property(s => s.RowVersion).IsRowVersion();
        entity.Property(s => s.CreatedAt).HasMaxLength(6).HasDefaultValue(DateTime.Now);
        entity.Property(s => s.UpdatedAt).HasMaxLength(6);

        entity.HasIndex(s => s.CorrelationId).IsUnique();
    }

}

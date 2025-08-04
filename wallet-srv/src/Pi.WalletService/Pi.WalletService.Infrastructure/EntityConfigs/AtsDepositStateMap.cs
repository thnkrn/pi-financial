using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pi.WalletService.Domain.AggregatesModel.AtsDepositAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Infrastructure.EntityConfigs;

public class AtsDepositStateMap : SagaClassMap<AtsDepositState>
{
    public AtsDepositStateMap()
    {
    }

    protected override void Configure(EntityTypeBuilder<AtsDepositState> entity, ModelBuilder model)
    {
        entity.Property(s => s.CorrelationId).HasMaxLength(36).IsRequired();
        entity.Property(s => s.CurrentState).HasMaxLength(100);
        entity.Property(s => s.Product).HasConversion(new EnumToStringConverter<Product>());
        entity.Property(s => s.Channel).HasConversion(new EnumToStringConverter<Channel>());
        entity.Property(s => s.OtpRequestRef);
        entity.Property(s => s.OtpRequestId);
        entity.Property(s => s.OtpConfirmedDateTime).HasMaxLength(6);
        entity.Property(s => s.PaymentReceivedAmount).HasPrecision(65, 30);
        entity.Property(s => s.PaymentReceivedDateTime).HasMaxLength(6);
        entity.Property(s => s.Fee).HasPrecision(65, 30);
        entity.Property(s => s.FailedReason);
        entity.Property(s => s.RequestId);
        entity.Property(s => s.RowVersion).IsRowVersion();
        entity.Property(s => s.CreatedAt).HasMaxLength(6);
        entity.Property(s => s.UpdatedAt).HasMaxLength(6);

        entity.HasIndex(s => s.CorrelationId).IsUnique();
    }
}

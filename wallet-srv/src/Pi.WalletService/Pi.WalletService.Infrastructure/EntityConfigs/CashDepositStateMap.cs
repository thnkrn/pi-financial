using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using CashDepositState = Pi.WalletService.Domain.AggregatesModel.CashAggregate.CashDepositState;

namespace Pi.WalletService.Infrastructure.EntityConfigs;

public class CashDepositStateMap : SagaClassMap<CashDepositState>
{
    public CashDepositStateMap()
    {
    }

    protected override void Configure(EntityTypeBuilder<CashDepositState> entity, ModelBuilder model)
    {
        entity.Property(d => d.CorrelationId).HasMaxLength(36).IsRequired();
        entity.Property(d => d.TransactionNo).HasMaxLength(36);
        entity.Property(x => x.UserId).HasMaxLength(36);
        entity.Property(x => x.AccountCode).HasMaxLength(36);
        entity.Property(x => x.CustomerCode).HasMaxLength(36);
        entity.Property(x => x.Channel).HasConversion(new EnumToStringConverter<Channel>()).IsRequired();
        entity.Property(x => x.Product).HasConversion(new EnumToStringConverter<Product>()).IsRequired();
        entity.Property(x => x.Purpose).HasConversion(new EnumToStringConverter<Purpose>()).IsRequired();
        entity.Property(x => x.CurrentState).HasMaxLength(100);
        entity.Property(x => x.RequestedAmount).HasPrecision(65, 30).IsRequired();
        entity.Property(x => x.CreatedAt).HasMaxLength(6);
        entity.Property(x => x.UpdatedAt).HasMaxLength(6);
        entity.Property(x => x.PaymentReceivedDateTime).HasMaxLength(6);
        entity.Property(d => d.BankName).HasMaxLength(50);
        entity.Property(d => d.FailedReason).HasMaxLength(200);
        entity.HasIndex(x => x.TransactionNo).IsUnique();
        entity.Property(x => x.RowVersion).IsRowVersion();
    }
}
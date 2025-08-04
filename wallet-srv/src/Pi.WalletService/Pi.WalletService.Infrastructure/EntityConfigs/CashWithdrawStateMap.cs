using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using CashWithdrawState = Pi.WalletService.Domain.AggregatesModel.CashAggregate.CashWithdrawState;

namespace Pi.WalletService.Infrastructure.EntityConfigs;

public class CashWithdrawStateMap : SagaClassMap<CashWithdrawState>
{
    public CashWithdrawStateMap()
    {
    }

    protected override void Configure(EntityTypeBuilder<CashWithdrawState> entity, ModelBuilder model)
    {
        entity.Property(d => d.CorrelationId).HasMaxLength(36).IsRequired();
        entity.Property(d => d.TransactionNo).HasMaxLength(36);
        entity.Property(x => x.UserId).HasMaxLength(36);
        entity.Property(x => x.AccountCode).HasMaxLength(36);
        entity.Property(x => x.CustomerCode).HasMaxLength(36);
        entity.Property(x => x.Channel).HasConversion(new EnumToStringConverter<Channel>()).IsRequired();
        entity.Property(x => x.Product).HasConversion(new EnumToStringConverter<Product>()).IsRequired();
        entity.Property(x => x.CurrentState).HasMaxLength(100);
        entity.Property(x => x.BankFee).HasPrecision(65, 30);
        entity.Property(x => x.CreatedAt).HasMaxLength(6);
        entity.Property(x => x.UpdatedAt).HasMaxLength(6);
        entity.Property(d => d.BankName).HasMaxLength(50);
        entity.Property(d => d.BankCode).HasMaxLength(10);
        entity.Property(d => d.BankAccountNo).HasMaxLength(36);
        entity.Property(d => d.FailedReason).HasMaxLength(200);
        entity.HasIndex(x => x.TransactionNo).IsUnique();
        entity.Property(x => x.RowVersion).IsRowVersion();
    }
}
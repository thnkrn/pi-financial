using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Infrastructure.EntityConfigs;

public class WithdrawEntrypointStateMap : SagaClassMap<WithdrawEntrypointState>
{
    public WithdrawEntrypointStateMap()
    {
    }

    protected override void Configure(EntityTypeBuilder<WithdrawEntrypointState> entity, ModelBuilder builder)
    {
        // Properties
        entity.Property(s => s.CorrelationId).HasMaxLength(36).IsRequired();
        entity.Property(s => s.CurrentState).HasMaxLength(100);
        entity.Property(s => s.TransactionNo).HasMaxLength(36);
        entity.Property(s => s.UserId).HasMaxLength(36);
        entity.Property(s => s.AccountCode).HasMaxLength(36);
        entity.Property(s => s.CustomerCode).HasMaxLength(36);
        entity.Property(s => s.Channel).HasConversion(new EnumToStringConverter<Channel>()).IsRequired();
        entity.Property(s => s.Product).HasConversion(new EnumToStringConverter<Product>()).IsRequired();
        entity.Property(s => s.Purpose).HasConversion(new EnumToStringConverter<Purpose>()).IsRequired();
        entity.Property(s => s.RequestedAmount).HasPrecision(65, 30).IsRequired();
        entity.Property(s => s.NetAmount).HasPrecision(65, 30);
        entity.Property(s => s.CustomerName).IsEncrypted().HasMaxLength(500);
        entity.Property(s => s.BankAccountName).IsEncrypted().HasMaxLength(500);
        entity.Property(s => s.BankAccountTaxId).IsEncrypted().HasMaxLength(100);
        entity.Property(s => s.BankAccountNo).HasMaxLength(36);
        entity.Property(s => s.BankBranchCode).HasMaxLength(10);
        entity.Property(s => s.BankName).HasMaxLength(50);
        entity.Property(s => s.BankCode).HasMaxLength(10);
        entity.Property(s => s.FailedReason);
        entity.Property(s => s.RequestId).HasMaxLength(36);
        entity.Property(s => s.RequesterDeviceId).HasMaxLength(36);
        entity.Property(s => s.RowVersion).IsRowVersion();
        entity.Property(s => s.CreatedAt).HasMaxLength(6);
        entity.Property(s => s.UpdatedAt).HasMaxLength(6);
        entity.Property(s => s.EffectiveDate).HasMaxLength(6);

        // Indexes
        entity.HasIndex(s => s.TransactionNo).IsUnique();
    }

}

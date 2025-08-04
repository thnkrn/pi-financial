using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.AggregatesModel.WalletAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Infrastructure.EntityConfigs;

public class TransactionHistoryMap : IEntityTypeConfiguration<TransactionHistory>
{
    public TransactionHistoryMap()
    {
    }

    public void Configure(EntityTypeBuilder<TransactionHistory> entity)
    {
        entity.Property(d => d.CorrelationId).HasMaxLength(36).IsRequired();
        entity.Property(d => d.TransactionNo).HasMaxLength(36).IsRequired();
        entity.Property(x => x.TransactionType).HasConversion(new EnumToStringConverter<TransactionType>());
        entity.Property(x => x.UserId).HasMaxLength(36);
        entity.Property(x => x.AccountCode).HasMaxLength(36);
        entity.Property(x => x.CustomerCode).HasMaxLength(36);
        entity.Property(x => x.GlobalAccount).HasMaxLength(36);
        entity.Property(x => x.Channel).HasConversion(new EnumToStringConverter<Channel>()).IsRequired();
        entity.Property(x => x.Product).HasConversion(new EnumToStringConverter<Product>()).IsRequired();
        entity.Property(x => x.Purpose).HasConversion(new EnumToStringConverter<Purpose>()).IsRequired();
        entity.Property(x => x.State).HasMaxLength(100).IsRequired();
        entity.Property(x => x.RequestedAmount).HasPrecision(65, 30).IsRequired();
        entity.Property(x => x.BankFee).HasPrecision(65, 30);
        entity.Property(x => x.CreatedAt).HasMaxLength(6).HasDefaultValue(DateTime.Now);
        entity.Property(x => x.TransactionDateTime).HasMaxLength(6);
        entity.Property(x => x.TransactionAmount).HasPrecision(65, 30);
        entity.Property(d => d.CustomerName).IsEncrypted();
        entity.Property(d => d.BankAccountName).IsEncrypted();
        entity.Property(d => d.BankName).HasMaxLength(50);
        entity.Property(d => d.BankCode).HasMaxLength(10);
        entity.Property(d => d.BankAccountNo).HasMaxLength(36);
        entity.Property(d => d.FailedReason).HasMaxLength(200);
    }
}
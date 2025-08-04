using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pi.User.Domain.AggregatesModel.TradeAccountAggregate;

namespace Pi.User.Infrastructure.EntityConfigs;

public class TradeAccountEntityConfig : IEntityTypeConfiguration<TradeAccount>
{
    public void Configure(EntityTypeBuilder<TradeAccount> builder)
    {
        builder.HasKey(t => t.Id);
        builder.HasIndex(t => t.UserAccountId);
        builder.Property(t => t.AccountNumber).HasMaxLength(64);
        builder.Property(t => t.AccountType).HasConversion(new EnumToStringConverter<AccountType>());
        builder.Property(t => t.AccountTypeCode).HasMaxLength(2);
        builder.Property(t => t.ExchangeMarketId).HasMaxLength(2);
        builder.Property(t => t.AccountStatus).HasMaxLength(10);
        builder.Property(t => t.CreditLine).HasPrecision(16, 2);
        builder.Property(t => t.CreditLineCurrency).HasMaxLength(3);
        builder.Property(t => t.MarketingId).HasMaxLength(10);
        builder.Property(t => t.SaleLicense).HasMaxLength(12);
        builder.Property(t => t.UserAccountId).HasMaxLength(10);

        builder.HasMany(t => t.ExternalAccounts)
            .WithOne()
            .HasForeignKey(t => t.TradeAccountId);
    }
}
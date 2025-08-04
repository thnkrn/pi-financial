using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pi.User.Domain.AggregatesModel.TradingAccountAggregate;

namespace Pi.User.Infrastructure.EntityConfigs;

public class TradingAccountV2EntityConfig : IEntityTypeConfiguration<TradingAccount>
{
    private readonly string _tableName;

    public TradingAccountV2EntityConfig(string tableName)
    {
        _tableName = tableName;
    }
    public void Configure(EntityTypeBuilder<TradingAccount> builder)
    {
        builder.HasNoKey().ToTable(_tableName);
        builder.HasIndex(o => new { o.CustomerCode, o.TradingAccountNo, o.AccountOpeningDate });
        builder.Property(o => o.TradingAccountNo)
            .HasMaxLength(15)
            .IsUnicode(false)
            .HasColumnName("account");
        builder.Property(o => o.CustomerCode)
            .HasMaxLength(8)
            .IsUnicode(false)
            .HasColumnName("custcode");
        builder.Property(o => o.AccountStatus)
            .HasMaxLength(1)
            .IsUnicode(false)
            .HasColumnName("acctstatus");
        builder.Property(o => o.MarketingId)
            .HasMaxLength(10)
            .IsUnicode(false)
            .HasColumnName("mktid");
        builder.Property(o => o.CreditLine)
            .HasColumnType("money")
            .HasColumnName("appcreditline");
        builder.Property(o => o.CreditLineEffectiveDate)
            .HasColumnType("date")
            .HasColumnName("lineeffective");
        builder.Property(o => o.CreditLineEndDate)
            .HasColumnType("date")
            .HasColumnName("lineexpire");
        builder.Property(o => o.AccountOpeningDate)
            .HasColumnType("date")
            .HasColumnName("effdate");
        builder.Property(o => o.AccountTypeCode)
            .HasMaxLength(1)
            .IsUnicode(false)
            .HasColumnName("custacct");
        builder.Property(o => o.ExchangeMarketId)
            .IsUnicode(false)
            .HasColumnName("xchgmkt");
    }
}
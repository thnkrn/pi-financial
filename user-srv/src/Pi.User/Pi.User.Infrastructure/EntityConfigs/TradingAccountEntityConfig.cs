using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pi.User.Domain.AggregatesModel.UserInfoAggregate;

namespace Pi.User.Infrastructure.EntityConfigs;

public class TradingAccountEntityConfig : IEntityTypeConfiguration<TradingAccount>
{
    public void Configure(EntityTypeBuilder<TradingAccount> builder)
    {
        builder
            .HasOne(p => p.UserInfo)
            .WithMany(b => b.TradingAccounts)
            .HasForeignKey(p => p.UserInfoId);
        builder.HasIndex(o => o.TradingAccountId);
    }
}
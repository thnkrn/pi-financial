using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pi.User.Domain.AggregatesModel.UserAccountAggregate;

namespace Pi.User.Infrastructure.EntityConfigs;

public class UserAccountEntityConfig : IEntityTypeConfiguration<UserAccount>
{
    public void Configure(EntityTypeBuilder<UserAccount> builder)
    {
        builder.HasKey(u => u.Id);
        builder.HasIndex(u => u.UserId);
        builder.Property(u => u.Id).HasMaxLength(10);
        builder.Property(u => u.UserAccountType).HasMaxLength(20);
        builder.Property(t => t.UserAccountType).HasConversion(new EnumToStringConverter<UserAccountType>());

        builder.HasMany(u => u.TradeAccounts)
            .WithOne()
            .HasForeignKey(t => t.UserAccountId);
    }
}
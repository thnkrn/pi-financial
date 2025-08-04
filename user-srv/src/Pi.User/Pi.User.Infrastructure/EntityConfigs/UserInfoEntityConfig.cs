using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pi.User.Domain.AggregatesModel.BankAccountAggregate;
using Pi.User.Domain.AggregatesModel.UserInfoAggregate;

namespace Pi.User.Infrastructure.EntityConfigs;

public class UserInfoEntityConfig : IEntityTypeConfiguration<UserInfo>
{
    public void Configure(EntityTypeBuilder<UserInfo> builder)
    {
        builder
            .Property(u => u.Email)
            .IsEncrypted();
        builder
            .HasIndex(u => u.CustomerId)
            .IsUnique();
        builder
            .Property(u => u.CitizenId)
            .IsEncrypted();
        builder
            .Property(u => u.FirstnameTh)
            .IsEncrypted();
        builder
            .Property(u => u.LastnameTh)
            .IsEncrypted();
        builder
            .Property(u => u.FirstnameEn)
            .IsEncrypted();
        builder
            .Property(u => u.LastnameEn)
            .IsEncrypted();
        builder
            .Property(u => u.PhoneNumber)
            .IsEncrypted();
        builder
            .HasIndex(c => c.PhoneNumberHash)
            .IsUnique();

        builder
            .Property(u => u.DateOfBirth)
            .IsEncrypted();

        builder.HasMany(u => u.BankAccountsV2)
            .WithOne()
            .HasForeignKey(b => b.UserId);
        builder.HasMany(u => u.SuitabilityTests)
            .WithOne()
            .HasForeignKey(s => s.UserId);
        builder.HasMany(b => b.UserAccounts)
            .WithOne()
            .HasForeignKey(u => u.UserId);
    }
}
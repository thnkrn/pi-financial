using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pi.User.Domain.AggregatesModel.BankAccountAggregate;

namespace Pi.User.Infrastructure.EntityConfigs;

public class BankAccountV2EntityConfig : IEntityTypeConfiguration<BankAccountV2>
{
    public void Configure(EntityTypeBuilder<BankAccountV2> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.AccountName).HasMaxLength(1000).IsEncrypted();
        builder.Property(b => b.AccountNo).HasMaxLength(1000).IsEncrypted();
        builder.Property(o => o.HashedAccountNo).HasMaxLength(64);
        builder.Property(b => b.BankCode).HasMaxLength(5);
        builder.Property(b => b.BranchCode).HasMaxLength(8);
        builder.Property(b => b.PaymentToken).HasMaxLength(100);

        builder.HasIndex(b => b.UserId);
        builder.HasIndex(b => new { b.HashedAccountNo, b.BankCode }).IsUnique();
    }
}
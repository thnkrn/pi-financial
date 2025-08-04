using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pi.User.Domain.AggregatesModel.TransactionIdAggregate;

namespace Pi.User.Infrastructure.EntityConfigs;

public class TransactionIdEntityConfig : IEntityTypeConfiguration<TransactionId>
{
    public void Configure(EntityTypeBuilder<TransactionId> builder)
    {
        builder.HasKey(o => new { o.Prefix, o.Date });
        builder.Property(o => o.Prefix).HasMaxLength(4);
        builder.Property(o => o.ReferId).HasMaxLength(16);
        builder.HasIndex(o => o.ReferId);
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pi.User.Domain.AggregatesModel.ExternalAccountAggregate;

namespace Pi.User.Infrastructure.EntityConfigs;

public class ExternalAccountEntityConfig : IEntityTypeConfiguration<ExternalAccount>
{
    public void Configure(EntityTypeBuilder<ExternalAccount> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Value).HasMaxLength(20);

        builder.HasIndex(e => new { e.TradeAccountId, e.ProviderId }).IsUnique();
    }
}
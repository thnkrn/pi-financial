using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pi.OnePort.Db2.Converters;
using Pi.OnePort.Db2.Models;

namespace Pi.OnePort.Db2.EntityConfigs;

public class DealMap : IEntityTypeConfiguration<DealOrder>, IEntityTypeConfiguration<AccountDeal>
{
    public void Configure(EntityTypeBuilder<DealOrder> builder)
    {
        builder.HasNoKey();
        builder.Property(q => q.DealTime).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.DealDate).HasConversion(new StringEmptyTrimConverter());
    }

    public void Configure(EntityTypeBuilder<AccountDeal> builder)
    {
        builder.HasNoKey();
        builder.Property(q => q.DealTime).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.DealDate).HasConversion(new StringEmptyTrimConverter());
    }
}

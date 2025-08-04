using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;

namespace Pi.SetService.Infrastructure.EntityConfigs;

public class EquityInitialMarginMap : IEntityTypeConfiguration<EquityInitialMargin>
{
    public void Configure(EntityTypeBuilder<EquityInitialMargin> builder)
    {
        builder.Property(x => x.MarginCode).HasMaxLength(255);
        builder.Property(x => x.CreatedAt).HasMaxLength(6).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
        builder.Property(x => x.UpdatedAt).HasMaxLength(6).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
        builder.Property(x => x.RowVersion).IsRowVersion();
        builder.HasIndex(x => new { x.MarginCode })
            .IsUnique()
            .HasDatabaseName("unique_margin_code");
    }
}

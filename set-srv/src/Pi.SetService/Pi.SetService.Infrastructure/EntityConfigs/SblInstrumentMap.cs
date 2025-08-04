using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;

namespace Pi.SetService.Infrastructure.EntityConfigs;

public class SblInstrumentMap : IEntityTypeConfiguration<SblInstrument>
{
    public void Configure(EntityTypeBuilder<SblInstrument> builder)
    {
        builder.Property(x => x.Symbol).HasMaxLength(64);
        builder.Property(x => x.CreatedAt).HasMaxLength(6).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
        builder.Property(x => x.UpdatedAt).HasMaxLength(6).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
        builder.HasIndex(x => new { x.Symbol });
    }
}

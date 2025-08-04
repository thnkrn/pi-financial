using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.SetService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.SetService.Infrastructure.EntityConfigs;

public class SblOrderMap : IEntityTypeConfiguration<SblOrder>
{
    public void Configure(EntityTypeBuilder<SblOrder> builder)
    {
        builder.Property(x => x.TradingAccountNo).HasMaxLength(36);
        builder.Property(x => x.CustomerCode).HasMaxLength(36);
        builder.Property(x => x.Symbol).HasMaxLength(64);
        builder.Property(x => x.Type).HasConversion(new EnumToStringConverter<SblOrderType>()).HasMaxLength(64);
        builder.Property(x => x.Status).HasConversion(new EnumToStringConverter<SblOrderStatus>()).HasMaxLength(64);
        builder.Property(x => x.CreatedAt).HasMaxLength(6).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
        builder.Property(x => x.UpdatedAt).HasMaxLength(6).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
        builder.Property(x => x.CreatedAtDate).HasComputedColumnSql("DATE(created_at)");
        builder.HasIndex(x => new { x.OrderId, x.CreatedAtDate })
            .IsUnique()
            .HasDatabaseName("unique_order_date");
        builder.HasIndex(x => new { x.TradingAccountNo });
        builder.HasIndex(x => new { x.Symbol });
        builder.HasIndex(x => new { x.Status });
    }
}

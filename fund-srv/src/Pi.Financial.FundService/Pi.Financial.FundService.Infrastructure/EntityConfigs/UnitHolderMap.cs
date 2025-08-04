using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.Financial.FundService.Infrastructure.EntityConfigs;

public class UnitHolderMap : IEntityTypeConfiguration<UnitHolder>
{
    public void Configure(EntityTypeBuilder<UnitHolder> builder)
    {
        builder.Property(x => x.AmcCode).HasMaxLength(36);
        builder.Property(x => x.TradingAccountNo).HasMaxLength(36);
        builder.Property(x => x.UnitHolderId).HasMaxLength(36);
        builder.Property(x => x.CustomerCode).HasMaxLength(36);
        builder.Property(x => x.UnitHolderType).HasConversion(new EnumToStringConverter<UnitHolderType>()).HasMaxLength(255);
        builder.Property(x => x.Status).HasConversion(new EnumToStringConverter<UnitHolderStatus>()).HasMaxLength(36);
        builder.Property(x => x.CreatedAt).HasMaxLength(6).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
        builder.HasIndex(x => x.UnitHolderId);
    }
}

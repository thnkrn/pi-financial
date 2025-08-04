using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pi.TfexService.Domain.Models.ActivitiesLog;
using Pi.TfexService.Domain.Models.InitialMargin;

namespace Pi.TfexService.Infrastructure.EntityConfigs;

public class InitialMarginMaps : IEntityTypeConfiguration<InitialMargin>
{
    public void Configure(EntityTypeBuilder<InitialMargin> entity)
    {
        entity.Property(s => s.Id).HasMaxLength(36).IsRequired();
        entity.Property(s => s.Symbol).HasMaxLength(36).IsRequired();
        entity.Property(s => s.ProductType).HasMaxLength(6).IsRequired();
        entity.Property(s => s.Im).HasPrecision(65, 30).IsRequired();
        entity.Property(s => s.ImOutright).HasPrecision(65, 30).IsRequired();
        entity.Property(s => s.ImSpread).HasPrecision(65, 30).IsRequired();
        entity.Property(s => s.AsOfDate).HasMaxLength(6).IsRequired();
        entity.Property(s => s.RowVersion).IsRowVersion();
        entity.Property(s => s.CreatedAt).HasColumnType("datetime").HasDefaultValueSql("NOW()");
        entity.Property(s => s.UpdatedAt).HasColumnType("datetime").HasDefaultValueSql("NOW()");

        entity.HasIndex(s => s.Symbol);
    }
}

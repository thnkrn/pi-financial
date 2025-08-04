using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pi.TfexService.Domain.Models.ActivitiesLog;

namespace Pi.TfexService.Infrastructure.EntityConfigs;

public class ActivitiesLogMap : IEntityTypeConfiguration<ActivitiesLog>
{
    public ActivitiesLogMap()
    {
    }

    public void Configure(EntityTypeBuilder<ActivitiesLog> entity)
    {
        entity.Property(s => s.Id).HasMaxLength(36).IsRequired();
        entity.Property(s => s.UserId).HasMaxLength(36);
        entity.Property(s => s.CustomerCode).HasMaxLength(36);
        entity.Property(s => s.AccountCode).HasMaxLength(36).IsRequired();
        entity.Property(s => s.RequestType).HasMaxLength(36).HasConversion<string>().IsRequired();
        entity.Property(s => s.OrderNo).HasMaxLength(36);
        entity.Property(s => s.RequestBody).IsEncrypted();
        entity.Property(s => s.ResponseBody).IsEncrypted();
        entity.Property(s => s.RequestedAt).HasMaxLength(6);
        entity.Property(s => s.CompletedAt).HasMaxLength(6);
        entity.Property(s => s.RowVersion).IsRowVersion();
        entity.Property(s => s.CreatedAt).HasColumnType("datetime").HasDefaultValueSql("NOW()");
        entity.Property(s => s.UpdatedAt).HasColumnType("datetime").HasDefaultValueSql("NOW()");
        entity.Property(s => s.IsSuccess);
        entity.Property(s => s.FailedReason).HasMaxLength(200);
        entity.Property(s => s.Symbol).HasMaxLength(36);
        entity.Property(s => s.Side).HasMaxLength(36);
        entity.Property(s => s.PriceType).HasMaxLength(36);
        entity.Property(s => s.Price).HasPrecision(65, 30);
        entity.Property(s => s.Qty);
        entity.Property(s => s.RejectCode).HasMaxLength(36);
        entity.Property(s => s.RejectReason).HasMaxLength(200);

        entity.HasIndex(s => s.OrderNo);
    }
}

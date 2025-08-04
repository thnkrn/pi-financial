using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pi.WalletService.Domain.AggregatesModel.RefundInfoAggregate;
namespace Pi.WalletService.Infrastructure.EntityConfigs;

public class RefundInfoMap : IEntityTypeConfiguration<RefundInfo>
{
    public RefundInfoMap()
    {
    }

    public void Configure(EntityTypeBuilder<RefundInfo> entity)
    {
        entity.Property(o => o.Id).HasMaxLength(36).IsRequired();
        entity.Property(o => o.Amount).HasPrecision(65, 30);
        entity.Property(o => o.TransferToAccountNo).HasMaxLength(20);
        entity.Property(o => o.TransferToAccountName).HasMaxLength(512).IsEncrypted();
        entity.Property(o => o.Fee).HasPrecision(65, 30);
        entity.Property(o => o.CurrentState).HasMaxLength(100);
        entity.Property(o => o.CreatedAt).HasMaxLength(6).HasDefaultValue(DateTime.Now);

        entity.HasIndex(o => o.Id);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pi.OnePort.Db2.Converters;
using Pi.OnePort.Db2.Models;

namespace Pi.OnePort.Db2.EntityConfigs;

public partial class OrderMap : IEntityTypeConfiguration<Order>, IEntityTypeConfiguration<OfflineOrder>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {

        builder.HasNoKey();
        builder.Property(q => q.AccountNo).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.CancelTime).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.SecSymbol).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.TrusteeId).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.Side).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.Condition).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.ConditionPrice).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.OrderType).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.EnterId).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.OrderDate).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.OrderTime).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.CancelId).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.CancelTime).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.ServiceType).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.OrderStatus).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.RejectCode).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.OrderToken).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.ControlKey).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.ValidTilDate).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.ExpireDate).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.MktOrdNo).HasConversion(new StringEmptyTrimConverter());
    }

    public void Configure(EntityTypeBuilder<OfflineOrder> builder)
    {
        builder.HasNoKey();
        builder.Property(q => q.AccountNo).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.CancelTime).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.SecSymbol).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.TrusteeId).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.Side).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.Condition).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.ConditionPrice).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.OrderType).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.EnterId).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.OrderDate).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.OrderTime).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.CancelId).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.CancelTime).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.ServiceType).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.OrderStatus).HasConversion(new StringTrimConverter());
        builder.Property(q => q.RejectCode).HasConversion(new StringTrimConverter());
        builder.Property(q => q.DelFlag).HasConversion(new StringTrimConverter());
    }
}

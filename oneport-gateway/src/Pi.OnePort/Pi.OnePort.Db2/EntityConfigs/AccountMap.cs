using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pi.OnePort.Db2.Converters;
using Pi.OnePort.Db2.Models;

namespace Pi.OnePort.Db2.EntityConfigs;

public class AccountMap : IEntityTypeConfiguration<AccountAvailable>,
    IEntityTypeConfiguration<AccountAvailableCreditBalance>,
    IEntityTypeConfiguration<AccountPosition>,
    IEntityTypeConfiguration<AccountPositionCreditBalance>
{
    public void Configure(EntityTypeBuilder<AccountAvailable> builder)
    {
        builder.HasNoKey();
        builder.Property(q => q.AccountNo).HasConversion(new StringTrimConverter());
        builder.Property(q => q.TraderId).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.CreditType).HasConversion(new StringEmptyTrimConverter());
    }

    public void Configure(EntityTypeBuilder<AccountAvailableCreditBalance> builder)
    {
        builder.HasNoKey();
        builder.Property(q => q.CustId).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.TraderId).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.Pc).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.Action).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.UpdateFlag).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.DelFlag).HasConversion(new StringEmptyTrimConverter());
    }

    public void Configure(EntityTypeBuilder<AccountPosition> builder)
    {
        builder.HasNoKey();
        builder.Property(q => q.AccountNo).HasConversion(new StringTrimConverter());
        builder.Property(q => q.SecSymbol).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.StockTypeChar).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.TrusteeId).HasConversion(new StringEmptyTrimConverter());
    }

    public void Configure(EntityTypeBuilder<AccountPositionCreditBalance> builder)
    {
        builder.HasNoKey();
        builder.Property(q => q.AccountNo).HasConversion(new StringTrimConverter());
        builder.Property(q => q.SecSymbol).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.StockTypeChar).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.TrusteeId).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.Grade).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.UpdateFlag).HasConversion(new StringEmptyTrimConverter());
        builder.Property(q => q.DelFlag).HasConversion(new StringEmptyTrimConverter());
    }
}

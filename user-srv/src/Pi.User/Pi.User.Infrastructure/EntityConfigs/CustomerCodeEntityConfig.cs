using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pi.User.Domain.AggregatesModel.UserInfoAggregate;

namespace Pi.User.Infrastructure.EntityConfigs;

public class CustomerCodeEntityConfig : IEntityTypeConfiguration<CustCode>
{
    public void Configure(EntityTypeBuilder<CustCode> builder)
    {
        builder
            .HasIndex(c => c.CustomerCode)
            .IsUnique();
        builder
            .HasOne(p => p.UserInfo)
            .WithMany(b => b.CustCodes)
            .HasForeignKey(p => p.UserInfoId);
    }
}
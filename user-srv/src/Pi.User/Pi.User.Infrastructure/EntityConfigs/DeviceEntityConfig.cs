using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pi.User.Domain.AggregatesModel.UserInfoAggregate;

namespace Pi.User.Infrastructure.EntityConfigs;

public class DeviceEntityConfig : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder
            .HasOne(p => p.UserInfo)
            .WithMany(b => b.Devices)
            .HasForeignKey(p => p.UserInfoId);
        builder
            .HasOne(d => d.NotificationPreference)
            .WithOne()
            .HasForeignKey<NotificationPreference>(n => n.DeviceForeignKey);
    }
}
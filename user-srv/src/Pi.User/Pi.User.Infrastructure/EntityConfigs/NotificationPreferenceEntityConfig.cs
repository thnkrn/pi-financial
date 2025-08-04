using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pi.User.Domain.AggregatesModel.UserInfoAggregate;

namespace Pi.User.Infrastructure.EntityConfigs;

public class NotificationPreferenceEntityConfig : IEntityTypeConfiguration<NotificationPreference>
{
    public void Configure(EntityTypeBuilder<NotificationPreference> builder)
    {
        builder
            .HasOne(p => p.UserInfo)
            .WithMany(b => b.NotificationPreferences)
            .HasForeignKey(p => p.UserInfoId);
    }
}
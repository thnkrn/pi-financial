using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pi.BackofficeService.Domain.AggregateModels.User;

namespace Pi.BackofficeService.Infrastructure.EntityConfig;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {

        builder.Property(u => u.FirstName).HasMaxLength(255).IsEncrypted();
        builder.Property(u => u.LastName).HasMaxLength(255).IsEncrypted();
        builder.Property(u => u.Email).HasMaxLength(255).IsEncrypted();

    }
}

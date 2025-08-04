using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pi.WalletService.Domain.AggregatesModel.LogAggregate;

namespace Pi.WalletService.Infrastructure.EntityConfigs;

public class FreewillRequestLogMap : IEntityTypeConfiguration<FreewillRequestLog>
{
    public FreewillRequestLogMap()
    {
    }

    public void Configure(EntityTypeBuilder<FreewillRequestLog> entity)
    {
        entity.Property(o => o.Request).IsRequired();
        entity.Property(x => x.Type).HasConversion(new EnumToStringConverter<FreewillRequestType>()).IsRequired();
        entity.Property(x => x.CreatedAt).HasMaxLength(6).HasDefaultValue(DateTime.Now);
    }
}
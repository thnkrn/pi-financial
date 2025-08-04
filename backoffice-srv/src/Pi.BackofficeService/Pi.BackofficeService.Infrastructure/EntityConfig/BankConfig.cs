using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;

namespace Pi.BackofficeService.Infrastructure.EntityConfig;

public class BankConfig : IEntityTypeConfiguration<Bank>, IEntityTypeConfiguration<BankChannel>
{
    public void Configure(EntityTypeBuilder<Bank> builder)
    {
        builder.HasNoKey();
        builder.Property(u => u.Code).HasMaxLength(255);
        builder.Property(u => u.Name);
        builder.Property(u => u.Abbreviation).HasMaxLength(255);

    }

    public void Configure(EntityTypeBuilder<BankChannel> builder)
    {
        builder.HasNoKey();
        builder.Property(u => u.BankCode).HasMaxLength(255);
        builder.Property(u => u.Channel).HasMaxLength(255);
    }
}

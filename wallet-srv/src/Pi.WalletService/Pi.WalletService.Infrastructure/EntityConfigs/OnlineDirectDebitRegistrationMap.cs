using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pi.WalletService.Domain.AggregatesModel.OnlineDirectDebitRegistrationAggregate;

namespace Pi.WalletService.Infrastructure.EntityConfigs;

public class OnlineDirectDebitRegistrationMap : IEntityTypeConfiguration<OnlineDirectDebitRegistration>
{
    public OnlineDirectDebitRegistrationMap()
    {
    }

    public void Configure(EntityTypeBuilder<OnlineDirectDebitRegistration> entity)
    {
        entity.HasKey(o => o.Id);
        entity.Property(o => o.Id).HasMaxLength(20);
        entity.Property(o => o.Bank).HasMaxLength(6);
        entity.Property(o => o.ExternalStatusCode).HasMaxLength(10);
    }
}
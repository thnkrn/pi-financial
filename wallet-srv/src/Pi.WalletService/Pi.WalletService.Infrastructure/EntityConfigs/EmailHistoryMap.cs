using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pi.WalletService.Domain.AggregatesModel.UtilityAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Infrastructure.EntityConfigs;

public class EmailHistoryMap : IEntityTypeConfiguration<EmailHistory>
{
    public EmailHistoryMap()
    {
    }

    public void Configure(EntityTypeBuilder<EmailHistory> entity)
    {
        entity.Property(d => d.Id).HasMaxLength(36).IsRequired();
        entity.Property(d => d.TicketId).HasMaxLength(36).IsRequired();
        entity.Property(d => d.TransactionNo).IsRequired();
        entity.Property(d => d.EmailType).HasConversion(new EnumToStringConverter<EmailType>()).IsRequired();
        entity.Property(d => d.SentAt).IsRequired();

        entity.HasIndex(d => d.TransactionNo);

    }
}
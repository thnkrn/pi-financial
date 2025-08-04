using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;

namespace Pi.BackofficeService.Infrastructure.EntityConfig;

public class TicketStateMap : SagaClassMap<TicketState>
{
    protected override void Configure(EntityTypeBuilder<TicketState> entity, ModelBuilder model)
    {
        entity.Property(x => x.Status).HasMaxLength(255).HasConversion(new EnumToStringConverter<Status>());
        entity.Property(x => x.RequestAction).HasMaxLength(255).HasConversion(new EnumToStringConverter<Method>());
        entity.Property(x => x.CheckerAction).HasMaxLength(255).HasConversion(new EnumToStringConverter<Method>());
        entity.Property(x => x.TransactionType).HasMaxLength(255).HasConversion(new EnumToStringConverter<TransactionType>());
        entity.Property(x => x.TransactionNo).HasMaxLength(255);
        entity.Property(x => x.TransactionState).HasMaxLength(255);
        entity.Property(x => x.TicketNo).HasMaxLength(255);
        entity.Property(x => x.CustomerName).HasMaxLength(255).IsEncrypted();
        entity.Property(x => x.Payload).IsEncrypted();
        entity.HasIndex(x => x.TicketNo).IsUnique();
    }
}

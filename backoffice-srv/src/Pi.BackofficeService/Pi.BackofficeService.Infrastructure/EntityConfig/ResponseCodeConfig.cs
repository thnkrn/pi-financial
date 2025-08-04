using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;

namespace Pi.BackofficeService.Infrastructure.EntityConfig;

public class ResponseCodeConfig : IEntityTypeConfiguration<ResponseCode>, IEntityTypeConfiguration<ResponseCodeAction>
{
    public void Configure(EntityTypeBuilder<ResponseCode> builder)
    {
        builder.Property(p => p.Machine)
            .HasConversion(new EnumToStringConverter<Machine>())
            .HasMaxLength(100);
        builder.Property(x => x.ProductType).HasMaxLength(100).HasConversion(new EnumToStringConverter<ProductType>());
        builder.Property(p => p.State).HasMaxLength(100);
        builder.Property(p => p.Suggestion).HasMaxLength(255);
        builder.Property(p => p.Description).HasMaxLength(255);
        builder.Property(p => p.IsFilterable);

        builder.HasMany(q => q.Actions)
            .WithOne(q => q.ResponseCode)
            .HasForeignKey(q => q.ResponseCodeId)
            .HasPrincipalKey(q => q.Id);
    }

    public void Configure(EntityTypeBuilder<ResponseCodeAction> builder)
    {
        builder.Property(p => p.Action).HasConversion(new EnumToStringConverter<Method>()).HasMaxLength(50);
    }
}

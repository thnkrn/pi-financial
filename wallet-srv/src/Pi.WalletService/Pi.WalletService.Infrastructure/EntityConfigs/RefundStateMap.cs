using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pi.WalletService.Domain.AggregatesModel.RefundAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Infrastructure.EntityConfigs;

public class RefundStateMap : SagaClassMap<RefundState>
{
    protected override void Configure(EntityTypeBuilder<RefundState> entity, ModelBuilder model)
    {
        entity.Property(d => d.CorrelationId).HasMaxLength(36).IsRequired();
        entity.Property(d => d.DepositTransactionNo).HasMaxLength(36);
        entity.Property(x => x.CurrentState).HasMaxLength(100);
        entity.Property(x => x.CustomerCode).HasMaxLength(100);
        entity.Property(x => x.TransactionNo).HasMaxLength(100);
        entity.Property(x => x.AccountCode).HasMaxLength(100);
        entity.Property(x => x.BankName).HasMaxLength(100);
        entity.Property(x => x.BankCode).HasMaxLength(100);
        entity.Property(x => x.BankAccountNo).HasMaxLength(100);
        entity.Property(x => x.UserId).HasMaxLength(100);
        entity.Property(x => x.Channel).HasConversion(new EnumToStringConverter<Channel>()).IsRequired();
        entity.Property(x => x.Product).HasConversion(new EnumToStringConverter<Product>()).IsRequired();
    }
}

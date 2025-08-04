using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletManualAllocationAggregate;
namespace Pi.WalletService.Infrastructure.EntityConfigs;

public class GlobalManualAllocationStateMap : SagaClassMap<GlobalManualAllocationState>
{
    protected override void Configure(EntityTypeBuilder<GlobalManualAllocationState> entity, ModelBuilder model)
    {
        entity.Property(x => x.Currency).HasConversion(new EnumToStringConverter<Currency>());
        entity.Property(x => x.RequestType).HasConversion(new EnumToStringConverter<GlobalManualAllocationType>());
        entity.Property(x => x.RowVersion).IsRowVersion().IsRequired();  // https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/1036#issuecomment-599666371
        entity.HasIndex(x => x.TransactionNo);
    }
}
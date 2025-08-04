using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Infrastructure.EntityConfigs;

public class GlobalWalletTransferStateMap : SagaClassMap<GlobalWalletTransferState>
{
    protected override void Configure(EntityTypeBuilder<GlobalWalletTransferState> entity, ModelBuilder model)
    {
        var currencyEnumToStringConverter = new EnumToStringConverter<Currency>();
        entity.Property(s => s.CorrelationId).HasMaxLength(36).IsRequired();
        entity.Property(s => s.TransactionNo).HasMaxLength(36);
        entity.Property(x => x.UserId).HasMaxLength(36);
        entity.Property(s => s.CustomerCode).HasMaxLength(36);
        entity.Property(s => s.Product).HasConversion(new EnumToStringConverter<Product>()).IsRequired();
        entity.Property(s => s.RequestedAmount).HasPrecision(65, 30).IsRequired();
        entity.Property(x => x.RequestedCurrency).HasConversion(currencyEnumToStringConverter);
        entity.Property(s => s.RequestedFxAmount).HasPrecision(65, 30).IsRequired();
        entity.Property(x => x.RequestedFxCurrency).HasConversion(currencyEnumToStringConverter);
        entity.Property(s => s.PaymentReceivedAmount).HasPrecision(65, 30);
        entity.Property(x => x.PaymentReceivedCurrency).HasConversion(currencyEnumToStringConverter);
        entity.Property(x => x.FxConfirmedCurrency).HasConversion(currencyEnumToStringConverter);
        entity.Property(x => x.TransferCurrency).HasConversion(currencyEnumToStringConverter);
        entity.Property(x => x.RowVersion).IsRowVersion().IsRequired();  // https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/1036#issuecomment-599666371
        entity.Property(x => x.TransactionType).HasConversion(new EnumToStringConverter<TransactionType>());
        entity.Property(x => x.Product).HasConversion(new EnumToStringConverter<Product>());
    }
}

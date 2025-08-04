using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pi.WalletService.Domain.AggregatesModel.GlobalTransfer;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Infrastructure.EntityConfigs;

public class GlobalTransferStateMap : SagaClassMap<GlobalTransferState>
{
    protected override void Configure(EntityTypeBuilder<GlobalTransferState> entity, ModelBuilder model)
    {
        var currencyEnumToStringConverter = new EnumToStringConverter<Currency>();
        entity.Property(s => s.CorrelationId).HasMaxLength(36).IsRequired();
        entity.Property(s => s.CurrentState).HasMaxLength(100);
        entity.Property(s => s.Product).HasConversion(new EnumToStringConverter<Product>());
        entity.Property(s => s.Channel).HasConversion(new EnumToStringConverter<Channel>());
        entity.Property(s => s.TransactionType).HasConversion(new EnumToStringConverter<TransactionType>());
        entity.Property(s => s.CustomerId).HasMaxLength(36);
        entity.Property(s => s.GlobalAccount);
        entity.Property(s => s.RequestedCurrency).HasConversion(currencyEnumToStringConverter);
        entity.Property(s => s.RequestedFxRate).HasPrecision(65, 30);
        entity.Property(s => s.FxMarkUpRate).HasPrecision(65, 30);
        entity.Property(s => s.RequestedFxCurrency).HasConversion(currencyEnumToStringConverter);
        entity.Property(s => s.ExchangeAmount).HasPrecision(65, 30);
        entity.Property(s => s.ExchangeCurrency).HasConversion(currencyEnumToStringConverter);
        entity.Property(s => s.FxConfirmedExchangeAmount).HasPrecision(65, 30);
        entity.Property(s => s.FxConfirmedExchangeCurrency).HasConversion(currencyEnumToStringConverter);
        entity.Property(s => s.ActualFxRate).HasPrecision(65, 30);
        entity.Property(s => s.FxInitiateRequestDateTime).HasMaxLength(6);
        entity.Property(s => s.FxTransactionId);
        entity.Property(s => s.FxConfirmedAmount).HasPrecision(65, 30);
        entity.Property(s => s.FxConfirmedExchangeRate);
        entity.Property(s => s.FxConfirmedCurrency).HasConversion(currencyEnumToStringConverter);
        entity.Property(s => s.FxConfirmedDateTime).HasMaxLength(6);
        entity.Property(s => s.TransferFromAccount);
        entity.Property(s => s.TransferAmount).HasPrecision(65, 30);
        entity.Property(s => s.TransferFee).HasPrecision(65, 30);
        entity.Property(s => s.TransferToAccount);
        entity.Property(s => s.TransferCurrency).HasConversion(currencyEnumToStringConverter);
        entity.Property(s => s.TransferRequestTime).HasMaxLength(6);
        entity.Property(s => s.TransferCompleteTime).HasMaxLength(6);
        entity.Property(s => s.FailedReason);
        entity.Property(s => s.RequestId);
        entity.Property(s => s.RowVersion).IsRowVersion().IsRequired();
        entity.Property(s => s.CreatedAt).HasMaxLength(6).HasDefaultValue(DateTime.Now);
        entity.Property(s => s.UpdatedAt).HasMaxLength(6);

        entity.HasIndex(s => s.CorrelationId).IsUnique();
    }
}

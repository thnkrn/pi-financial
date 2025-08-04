using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.Financial.FundService.Infrastructure.EntityConfigs
{
    public class FundOrderStateMap : SagaClassMap<FundOrderState>
    {
        protected override void Configure(EntityTypeBuilder<FundOrderState> entity, ModelBuilder model)
        {
            entity.Property(x => x.CurrentState).HasMaxLength(64);
            entity.Property(x => x.OrderNo).HasMaxLength(36);
            entity.Property(x => x.BrokerOrderId).HasMaxLength(36);
            entity.Property(x => x.AmcOrderId).HasMaxLength(36);
            entity.Property(x => x.TradingAccountNo).HasMaxLength(36);
            entity.Property(x => x.TradingAccountId).HasMaxLength(36);
            entity.Property(x => x.CustomerCode).HasMaxLength(36);
            entity.Property(x => x.FundCode).HasMaxLength(36);
            entity.Property(x => x.CounterFundCode).HasMaxLength(36);
            entity.Property(x => x.SaleLicense).HasMaxLength(36);
            entity.Property(x => x.BankAccount).HasMaxLength(255).IsEncrypted();
            entity.Property(x => x.SettlementBankAccount).HasMaxLength(255).IsEncrypted();
            entity.Property(x => x.BankCode).HasMaxLength(255).IsEncrypted();
            entity.Property(x => x.SettlementBankCode).HasMaxLength(255).IsEncrypted();
            entity.Property(x => x.Unit).HasPrecision(65, 30);
            entity.Property(x => x.Amount).HasPrecision(65, 30);
            entity.Property(x => x.AllottedUnit).HasPrecision(65, 30);
            entity.Property(x => x.AllottedAmount).HasPrecision(65, 30);
            entity.Property(x => x.AllottedNav).HasPrecision(65, 30);
            entity.Property(x => x.OrderSide).HasConversion(new EnumToStringConverter<OrderSide>()).HasMaxLength(255);
            entity.Property(x => x.RedemptionType).HasConversion(new EnumToStringConverter<RedemptionType>()).HasMaxLength(255);
            entity.Property(x => x.OrderType).HasConversion(new EnumToStringConverter<FundOrderType>()).HasMaxLength(255);
            entity.Property(x => x.OrderStatus).HasConversion(new EnumToStringConverter<FundOrderStatus>()).HasMaxLength(255);
            entity.Property(x => x.Channel).HasConversion(new EnumToStringConverter<Channel>()).HasMaxLength(255);
            entity.Property(x => x.AccountType).HasConversion(new EnumToStringConverter<FundAccountType>()).HasMaxLength(255);
            entity.Property(x => x.PaymentType).HasConversion(new EnumToStringConverter<PaymentType>()).HasMaxLength(255);
            entity.Property(x => x.CreatedAt).HasMaxLength(6).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            entity.Property(x => x.UpdatedAt).HasMaxLength(6).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            entity.HasIndex(x => x.UnitHolderId);
            entity.HasIndex(x => new { x.OrderNo, x.OrderType }).IsUnique();
            entity.HasIndex(x => new { x.BrokerOrderId, x.OrderType }).IsUnique();
        }
    }
}

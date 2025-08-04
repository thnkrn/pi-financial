using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.SetService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.SetService.Infrastructure.EntityConfigs
{
    public class EquityOrderStateMap : SagaClassMap<EquityOrderState>
    {
        protected override void Configure(EntityTypeBuilder<EquityOrderState> entity, ModelBuilder model)
        {
            entity.Property(x => x.CurrentState).HasMaxLength(64);
            entity.Property(x => x.OrderNo).HasMaxLength(36);
            entity.Property(x => x.BrokerOrderId).HasMaxLength(36);
            entity.Property(x => x.TradingAccountNo).HasMaxLength(36);
            entity.Property(x => x.CustomerCode).HasMaxLength(36);
            entity.Property(x => x.EnterId).HasMaxLength(10);
            entity.Property(x => x.OrderNo).HasMaxLength(64);
            entity.Property(x => x.BrokerOrderId).HasMaxLength(64);
            entity.Property(x => x.Price).HasMaxLength(64);
            entity.Property(x => x.SecSymbol).HasMaxLength(64);
            entity.Property(x => x.Volume);
            entity.Property(x => x.PubVolume);
            entity.Property(x => x.MatchedVolume);
            entity.Property(x => x.TodaySell);
            entity.Property(x => x.StockType).HasMaxLength(64);
            entity.Property(x => x.TradingAccountType).HasConversion(new EnumToStringConverter<TradingAccountType>()).HasMaxLength(64);
            entity.Property(x => x.OrderStatus).HasConversion(new EnumToStringConverter<OrderStatus>()).HasMaxLength(64);
            entity.Property(x => x.ServiceType).HasConversion(new EnumToStringConverter<ServiceType>()).HasMaxLength(64);
            entity.Property(x => x.ConditionPrice).HasConversion(new EnumToStringConverter<ConditionPrice>()).HasMaxLength(64);
            entity.Property(x => x.OrderAction).HasConversion(new EnumToStringConverter<OrderAction>()).HasMaxLength(64);
            entity.Property(x => x.OrderSide).HasConversion(new EnumToStringConverter<OrderSide>()).HasMaxLength(64);
            entity.Property(x => x.OrderType).HasConversion(new EnumToStringConverter<OrderType>()).HasMaxLength(64);
            entity.Property(x => x.Condition).HasConversion(new EnumToStringConverter<Condition>()).HasMaxLength(64);
            entity.Property(x => x.Ttf).HasConversion(new EnumToStringConverter<Ttf>()).HasMaxLength(64);
            entity.Property(x => x.IpAddress).HasMaxLength(64);
            entity.Property(x => x.CreatedAt).HasMaxLength(6).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            entity.Property(x => x.UpdatedAt).HasMaxLength(6).HasDefaultValueSql("CURRENT_TIESTAMP(6)");
            entity.Property(x => x.CreatedAtDate).HasComputedColumnSql("DATE(created_at)");
            entity.Property(x => x.RowVersion).IsRowVersion();
            entity.HasIndex(e => new { e.OrderNo, e.CreatedAtDate })
                .IsUnique()
                .HasDatabaseName("unique_order_date");
        }
    }
}

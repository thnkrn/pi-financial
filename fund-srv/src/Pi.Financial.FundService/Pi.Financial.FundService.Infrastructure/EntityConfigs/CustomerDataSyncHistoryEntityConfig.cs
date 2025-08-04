using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pi.Financial.FundService.Domain.AggregatesModel.CustomerDataAggregate;

namespace Pi.Financial.FundService.Infrastructure.EntityConfigs
{
    public class FundBankAccountUpdateStateMap : IEntityTypeConfiguration<CustomerDataSyncHistory>
    {
        public void Configure(EntityTypeBuilder<CustomerDataSyncHistory> builder)
        {
            builder.HasKey(x => x.CorrelationId);
            builder.Property(x => x.CustomerCode).HasMaxLength(10);
            builder.Property(x => x.RowVersion).IsRowVersion().IsRequired();
        }
    }
}

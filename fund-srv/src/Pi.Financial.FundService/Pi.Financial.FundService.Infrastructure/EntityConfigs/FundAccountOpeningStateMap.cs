using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pi.Financial.FundService.Domain.AggregatesModel.AccountOpeningAggregate;

namespace Pi.Financial.FundService.Infrastructure.EntityConfigs
{
    public class FundAccountOpeningStateMap : SagaClassMap<FundAccountOpeningState>
    {
        protected override void Configure(EntityTypeBuilder<FundAccountOpeningState> entity, ModelBuilder model)
        {
            entity.Property(x => x.CurrentState).HasMaxLength(64);
            entity.Property(x => x.RowVersion).IsRowVersion().IsRequired();  // https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/1036#issuecomment-599666371
            entity.Property(x => x.CustomerCode).HasMaxLength(10);
        }
    }
}

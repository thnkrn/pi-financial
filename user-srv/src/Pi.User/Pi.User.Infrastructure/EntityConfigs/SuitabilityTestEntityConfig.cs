using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pi.User.Domain.AggregatesModel.SuitabilityTestAggregate;

namespace Pi.User.Infrastructure.EntityConfigs;

public class SuitabilityTestEntityConfig : IEntityTypeConfiguration<SuitabilityTest>
{
    public void Configure(EntityTypeBuilder<SuitabilityTest> builder)
    {
        builder.HasKey(s => s.Id);
        builder.HasIndex(k => k.UserId);
        builder.Property(o => o.Grade).HasMaxLength(1);
        builder.Property(o => o.Version).HasMaxLength(10);
        builder.Property(o => o.ReviewDate).HasColumnType("date");
        builder.Property(o => o.ExpiredDate).HasColumnType("date");
    }
}
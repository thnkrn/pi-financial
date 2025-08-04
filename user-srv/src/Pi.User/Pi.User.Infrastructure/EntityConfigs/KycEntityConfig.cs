using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pi.User.Domain.AggregatesModel.KycAggregate;

namespace Pi.User.Infrastructure.EntityConfigs;

public class KycEntityConfig : IEntityTypeConfiguration<Kyc>
{
    public void Configure(EntityTypeBuilder<Kyc> builder)
    {
        builder.HasKey(k => k.Id);
        builder.HasIndex(k => k.UserId);
        builder.Property(k => k.ReviewDate).HasColumnType("date");
        builder.Property(k => k.ExpiredDate).HasColumnType("date");
    }
}
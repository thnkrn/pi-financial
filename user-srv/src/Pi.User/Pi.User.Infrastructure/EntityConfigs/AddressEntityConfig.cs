using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pi.User.Domain.AggregatesModel.AddressAggregate;

namespace Pi.User.Infrastructure.EntityConfigs;

public class AddressEntityConfig : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).ValueGeneratedNever();
        builder.Property(o => o.Place).HasMaxLength(1000).IsEncrypted();
        builder.Property(o => o.HomeNo).HasMaxLength(1000).IsEncrypted();
        builder.Property(o => o.Town).HasMaxLength(1000).IsEncrypted();
        builder.Property(o => o.Building).HasMaxLength(1000).IsEncrypted();
        builder.Property(o => o.Village).HasMaxLength(1000).IsEncrypted();
        builder.Property(o => o.Floor).HasMaxLength(1000).IsEncrypted();
        builder.Property(o => o.Soi).HasMaxLength(1000).IsEncrypted();
        builder.Property(o => o.Road).HasMaxLength(1000).IsEncrypted();
        builder.Property(o => o.SubDistrict).HasMaxLength(1000).IsEncrypted();
        builder.Property(o => o.District).HasMaxLength(1000).IsEncrypted();
        builder.Property(o => o.Province).HasMaxLength(1000).IsEncrypted();
        builder.Property(o => o.Country).HasMaxLength(1000).IsEncrypted();
        builder.Property(o => o.ZipCode).HasMaxLength(10);
        builder.Property(o => o.CountryCode).HasMaxLength(3);
        builder.Property(o => o.ProvinceCode).HasMaxLength(10);
        builder.HasIndex(o => o.UserId);
    }
}
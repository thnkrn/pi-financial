using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pi.WalletService.Domain.AggregatesModel.UtilityAggregate;

namespace Pi.WalletService.Infrastructure.EntityConfigs;

public class HolidayEntityConfiguration : IEntityTypeConfiguration<Holiday>
{
    public HolidayEntityConfiguration()
    {
    }

    public void Configure(EntityTypeBuilder<Holiday> entity)
    {
        entity.HasKey(o => o.HolidayDate);
        entity.Property(o => o.HolidayDate).IsRequired();
        entity.Property(o => o.HolidayName).HasMaxLength(150);
        entity.HasData(HolidaysData());
    }

    private List<Holiday> HolidaysData()
    {
        return new List<Holiday>
        {
            new (new DateOnly(2024, 01, 01), "New Year's Day"),
            new (new DateOnly(2024, 01, 02), "Substitution for New Year’s Eve (Sunday 31st December 2023) (cancelled)"),
            new (new DateOnly(2024, 02, 26), "Substitution for Makha Bucha Day (Saturday 24th February 2024)"),
            new (new DateOnly(2024, 04, 08), "Substitution for Chakri Memorial Day (Saturday 6th April 2024)"),
            new (new DateOnly(2024, 04, 12), "Additional special holiday (added)"),
            new (new DateOnly(2024, 04, 15), "Songkran Festival"),
            new (new DateOnly(2024, 04, 16), "Substitution for Songkran Festival (Saturday 13th April 2024 and Sunday 14th April 2024)"),
            new (new DateOnly(2024, 05, 01), "National Labour Day"),
            new (new DateOnly(2024, 05, 06), "Substitution for Coronation Day (Saturday 4th May 2024)"),
            new (new DateOnly(2024, 05, 22), "Visakha Bucha Day"),
            new (new DateOnly(2024, 06, 03), "H.M. Queen Suthida Bajrasudhabimalalakshana’s Birthday"),
            new (new DateOnly(2024, 07, 22), "Substitution for Asarnha Bucha Day (Saturday 20th July 2024)"),
            new (new DateOnly(2024, 07, 29), "Substitution for H.M. King Maha Vajiralongkorn Phra Vajiraklaochaoyuhua’s Birthday (Sunday 28th July 2024)"),
            new (new DateOnly(2024, 08, 12), "H.M. Queen Sirikit The Queen Mother’s Birthday / Mother’s Day"),
            new (new DateOnly(2024, 10, 14), "Substitution for H.M. King Bhumibol Adulyadej The Great Memorial Day (Sunday 13th October 2024)"),
            new (new DateOnly(2024, 10, 23), "H.M. King Chulalongkorn The Great Memorial Day"),
            new (new DateOnly(2024, 12, 05), "H.M. King Bhumibol Adulyadej The Great’s Birthday / National Day / Father’s Day"),
            new (new DateOnly(2024, 12, 10), "Constitution Day"),
            new (new DateOnly(2024, 12, 31), "New Year’s Eve")
        };
    }
}
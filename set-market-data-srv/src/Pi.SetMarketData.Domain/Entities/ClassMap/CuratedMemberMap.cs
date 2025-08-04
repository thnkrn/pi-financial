using CsvHelper.Configuration;
using Pi.SetMarketData.Domain.Converters;

namespace Pi.SetMarketData.Domain.Entities.ClassMap;

public sealed class CuratedMemberMap : ClassMap<CuratedMember>
{
    public CuratedMemberMap()
    {
        Map(m => m.CuratedListId).Name("curated_list_id");
        Map(m => m.InstrumentId).Name("instrument_id");
        Map(m => m.Symbol).Name("symbol");
        Map(m => m.Ordering).Name("ordering");
        Map(m => m.IsDefault).Name("is_default").TypeConverter<NullableIntConverter>();
        Map(m => m.GroupName).Name("group_name");
        Map(m => m.SubGroupName).Name("sub_group_name");
    }
}
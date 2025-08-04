using CsvHelper.Configuration;
using Pi.CsvDataImporter.Converters;
using Pi.CsvDataImporter.Models;

namespace Pi.CsvDataImporter.Mappers;

public sealed class CuratedMemberMap : ClassMap<CuratedMember>
{
    public CuratedMemberMap()
    {
        Map(m => m.CuratedListId).Name("curated_list_id");
        Map(m => m.InstrumentId).Name("instrument_id");
        Map(m => m.Ordering).Name("ordering");
        Map(m => m.IsDefault).Name("is_default").TypeConverter<CustomBooleanConverter>();
        Map(m => m.GroupName).Name("group_name");
        Map(m => m.SubGroupName).Name("sub_group_name");
        Map(m => m.Symbol).Name("symbol");
    }
}
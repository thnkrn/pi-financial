using CsvHelper.Configuration;
using Pi.CsvDataImporter.Converters;
using Pi.CsvDataImporter.Models;

namespace Pi.CsvDataImporter.Mappers;

public sealed class CuratedListMap : ClassMap<CuratedList>
{
    public CuratedListMap()
    {
        Map(m => m.CuratedListId).Name("curated_list_id");
        Map(m => m.CuratedListCode).Name("curated_list_code");
        Map(m => m.CuratedType).Name("curated_type");
        Map(m => m.RelevantTo).Name("relevant_to");
        Map(m => m.Name).Name("name");
        Map(m => m.Hashtag).Name("hashtag");
        Map(m => m.Ordering).Name("ordering").TypeConverter<CustomFloatConverter>();
        Map(m => m.CreateTime).Name("create_time").TypeConverter<CustomDateTimeConverter>();
        Map(m => m.UpdateTime).Name("update_time").TypeConverter<CustomDateTimeConverter>();
        Map(m => m.UpdateBy).Name("update_by");
        Map(m => m.IsDefault).Name("Is_default").TypeConverter<CustomBooleanConverter>();
    }
}
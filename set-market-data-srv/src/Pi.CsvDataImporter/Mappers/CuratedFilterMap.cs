using CsvHelper.Configuration;
using Pi.CsvDataImporter.Converters;
using Pi.CsvDataImporter.Models;

namespace Pi.CsvDataImporter.Mappers;

public sealed class CuratedFilterMap : ClassMap<CuratedFilter>
{
    public CuratedFilterMap()
    {
        Map(m => m.FilterId).Name("filter_id");
        Map(m => m.FilterName).Name("filter_name");
        Map(m => m.FilterCategory).Name("filter_category");
        Map(m => m.FilterType).Name("filter_type");
        Map(m => m.CategoryPriority).Name("category_priority");
        Map(m => m.GroupName).Name("group_name");
        Map(m => m.SubGroupName).Name("sub_group_name");
        Map(m => m.CuratedListId).Name("curated_list_id");
        Map(m => m.IsDefault).Name("is_default").TypeConverter<CustomBooleanConverter>();
        Map(m => m.Highlight).Name("highlight").TypeConverter<CustomBooleanConverter>();
        Map(m => m.Ordering).Name("ordering");
    }
}
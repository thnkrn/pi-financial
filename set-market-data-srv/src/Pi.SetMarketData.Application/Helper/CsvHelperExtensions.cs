using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Domain.Entities.ClassMap;

namespace Pi.SetMarketData.Application.Helper;

public static class CsvHelperExtensions
{
    public static IEnumerable<TEntity> ReadCsvData<TEntity>(Stream data)
    {
        using var reader = new StreamReader(data);

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = args => args.Header.ToLower(),
        };

        using var csv = new CsvReader(reader, config);

        if (typeof(TEntity) == typeof(CuratedList))
        {
            csv.Context.RegisterClassMap<CuratedListMap>();
        }
        else if (typeof(TEntity) == typeof(CuratedFilter))
        {
            csv.Context.RegisterClassMap<CuratedFilterMap>();
        }
        else if (typeof(TEntity) == typeof(CuratedMember))
        {
            csv.Context.RegisterClassMap<CuratedMemberMap>();
        }

        return [.. csv.GetRecords<TEntity>()];
    }
}
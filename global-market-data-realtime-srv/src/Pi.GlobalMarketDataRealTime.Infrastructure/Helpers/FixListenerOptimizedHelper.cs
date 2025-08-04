using System.Globalization;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Pi.GlobalMarketDataRealTime.Domain.Models.Fix;
using QuickFix.Fields;
using QuickFix.FIX44;

namespace Pi.GlobalMarketDataRealTime.Infrastructure.Helpers;

public static class FixListenerOptimizedHelper
{
    private static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;
    private static readonly string[] TimeFormats = ["HH:mm:ss.fff"];

    private static readonly MemoryCache DateCache = new(new MemoryCacheOptions
    {
        SizeLimit = 1000
    });

    private static readonly MemoryCache TimeCache = new(new MemoryCacheOptions
    {
        SizeLimit = 100000,
        CompactionPercentage = 0.2
    });

    public static List<MarketDataEntry> ExtractMarketDataEntries(MarketDataSnapshotFullRefresh message,
        MarketDataSnapshotFullRefresh.NoMDEntriesGroup group)
    {
        var totalEntries = message.GetInt(Tags.NoMDEntries);
        var entries = new List<MarketDataEntry>(totalEntries); // Preallocate list size

        for (var i = 1; i <= totalEntries; i++)
        {
            message.GetGroup(i, group);

            var marketDataEntry = CreateMarketDataEntry(group);
            if (marketDataEntry != null)
                entries.Add(marketDataEntry);
        }

        return entries;
    }

    public static MarketDataEntry? CreateMarketDataEntry(MarketDataSnapshotFullRefresh.NoMDEntriesGroup group)
    {
        var hasRequiredFields = group.IsSetField(Tags.MDEntryType);
        if (!hasRequiredFields)
            return null;

        var entry = new MarketDataEntry();
        var entryType = group.GetString(Tags.MDEntryType);

        if (entryType.Equals("J", StringComparison.OrdinalIgnoreCase)) return null;
        if (entryType.Equals("Y", StringComparison.OrdinalIgnoreCase)) return null;

        if (group.IsSetField(Tags.MDEntryType))
            entry.MdEntryType = entryType;

        if (group.IsSetField(Tags.MDEntryPx))
            entry.MdEntryPx = group.GetDecimal(Tags.MDEntryPx);

        if (group.IsSetField(Tags.MDEntrySize))
            entry.MdEntrySize = group.GetDecimal(Tags.MDEntrySize);

        if (group.IsSetField(Tags.MDEntryDate))
            entry.MdEntryDate = ParseDate(group.GetString(Tags.MDEntryDate));

        if (group.IsSetField(Tags.MDEntryTime))
            entry.MdEntryTime = ParseTime(group.GetString(Tags.MDEntryTime));

        return entry;
    }

    public static DateTime ParseDate(string dateString)
    {
        if (string.IsNullOrEmpty(dateString) || dateString.Length != 8)
            return DateTime.MinValue;

        if (!DateCache.TryGetValue(dateString, out DateTime cached))
        {
            try
            {
                var year = int.Parse(dateString[..4]);
                var month = int.Parse(dateString.Substring(4, 2));
                var day = int.Parse(dateString.Substring(6, 2));

                cached = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
            }
            catch
            {
                cached = DateTime.MinValue;
            }

            DateCache.Set(dateString, cached, new MemoryCacheEntryOptions
            {
                Size = 1,
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12)
            });
        }

        return cached;
    }

    public static DateTime ParseTime(string timeString)
    {
        if (string.IsNullOrEmpty(timeString) || timeString.Length < 8)
            return DateTime.MinValue;

        if (!TimeCache.TryGetValue(timeString, out DateTime cached))
        {
            try
            {
                cached = DateTime.ParseExact(timeString, TimeFormats,
                    InvariantCulture, DateTimeStyles.None);
            }
            catch
            {
                cached = DateTime.MinValue;
            }

            TimeCache.Set(timeString, cached, new MemoryCacheEntryOptions
            {
                Size = 1,
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });
        }

        return cached;
    }

    public static int GetNoRelatedSym(SecurityList message)
    {
        return message.IsSetField(Tags.NoRelatedSym) ? message.GetInt(Tags.NoRelatedSym) : 0;
    }

    public static string? GetFieldValueOrDefault(SecurityList.NoRelatedSymGroup group, int tag)
    {
        return group.IsSetField(tag) ? group.GetString(tag) : null;
    }

    public static decimal GetDecimalFieldValueOrDefault(SecurityList.NoRelatedSymGroup group, int tag)
    {
        return group.IsSetField(tag) ? group.GetDecimal(tag) : 0;
    }

    public static string SerializeSecurities(List<SecurityInfo> securities)
    {
        try
        {
            return JsonConvert.SerializeObject(securities);
        }
        catch
        {
            return "[]";
        }
    }

    public static SecurityInfo CreateSecurityInfo(SecurityList.NoRelatedSymGroup group)
    {
        try
        {
            return new SecurityInfo
            {
                SymbolId = group.GetString(Tags.Symbol),
                ISIN = GetFieldValueOrDefault(group, Tags.SecurityID),
                Exchange = GetFieldValueOrDefault(group, Tags.SecurityExchange),
                Currency = GetFieldValueOrDefault(group, Tags.Currency),
                SymbolType = GetFieldValueOrDefault(group, Tags.SecurityType),
                Description = GetFieldValueOrDefault(group, Tags.SecurityDesc),
                UnderlyingSymbolId = GetFieldValueOrDefault(group, Tags.UnderlyingSymbol),
                Country = GetFieldValueOrDefault(group, Tags.CountryOfIssue),
                MinPriceIncrement = GetDecimalFieldValueOrDefault(group, Tags.MinPriceIncrement),
                Ticker = GetFieldValueOrDefault(group, Tags.Symbol),
                Name = GetFieldValueOrDefault(group, 10001) // Assuming 10001 is the custom tag for Name
            };
        }
        catch
        {
            return new SecurityInfo();
        }
    }
}
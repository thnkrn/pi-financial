using System.Globalization;
using Newtonsoft.Json;
using Pi.GlobalMarketDataRealTime.DataHandler.Exceptions;
using Pi.GlobalMarketDataRealTime.Domain.Models.Fix;
using QuickFix.Fields;
using QuickFix.FIX44;

namespace Pi.GlobalMarketDataRealTime.DataHandler.Helpers;

public static class FixListenerHelper
{
    public static List<MarketDataEntry> ExtractMarketDataEntries(MarketDataSnapshotFullRefresh message,
        MarketDataSnapshotFullRefresh.NoMDEntriesGroup group)
    {
        var entries = new List<MarketDataEntry>();
        for (var i = 1; i <= message.GetInt(Tags.NoMDEntries); i++)
        {
            message.GetGroup(i, group);

            var entryType = group.GetString(Tags.MDEntryType);

            // FIX: Validate MDEntryType (269)
            if (entryType.Equals("J", StringComparison.OrdinalIgnoreCase)) // Invalid entry
                continue;

            entries.Add(CreateMarketDataEntry(group));
        }

        return entries;
    }

    public static MarketDataEntry CreateMarketDataEntry(MarketDataSnapshotFullRefresh.NoMDEntriesGroup group)
    {
        var entry = new MarketDataEntry();

        #region [OUM Arcadia] 20-Jun-24 Tag Checking

        // check tag 269
        if (group.Any(e => e.Key == Tags.MDEntryType))
        {
            var groupObj = group.Select(e => e);
            entry.MdEntryType = groupObj.FirstOrDefault(e => e.Key.ToString() == Tags.MDEntryType.ToString()).Value
                ?.ToString() ?? string.Empty;
        }

        // check tag 270
        if (group.Any(e => e.Key == Tags.MDEntryPx))
            entry.MdEntryPx = group.GetDecimal(Tags.MDEntryPx);

        // check tag 271
        if (group.Any(e => e.Key == Tags.MDEntrySize))
            entry.MdEntrySize = group.GetDecimal(Tags.MDEntrySize);

        // check tag 272
        if (group.Any(e => e.Key == Tags.MDEntryDate))
        {
            var dateString = group.GetString(Tags.MDEntryDate);
            entry.MdEntryDate = ParseDate(dateString);
        }

        // check tag 273
        if (group.Any(e => e.Key == Tags.MDEntryTime))
        {
            var timeString = group.GetString(Tags.MDEntryTime);
            entry.MdEntryTime = ParseTime(timeString);
        }

        #endregion

        return entry;
    }

    public static DateTime ParseDate(string dateString)
    {
        if (DateTime.TryParseExact(dateString, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out var parsedDate))
            return parsedDate;
        throw new SubscriptionServiceException($"Could not convert string ({dateString}) to DateTime");
    }

    public static DateTime ParseTime(string timeString)
    {
        if (DateTime.TryParseExact(timeString, "HH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out var parsedTime))
            return parsedTime;
        throw new SubscriptionServiceException($"Could not convert string ({timeString}) to DateTime");
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
        return JsonConvert.SerializeObject(securities);
    }

    public static SecurityInfo CreateSecurityInfo(SecurityList.NoRelatedSymGroup group)
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
}
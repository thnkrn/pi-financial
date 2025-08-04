using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Pi.SetMarketDataWSS.Application.Helpers;

public static class OrderBookStateMappingsHelper
{
    private static readonly Regex StateNameRegex = new Regex("_\\w",
        RegexOptions.Compiled,
        matchTimeout: TimeSpan.FromSeconds(2));

    private static readonly ImmutableHashSet<string> MarketSegmentDayMarket = ImmutableHashSet.Create(
        StringComparer.OrdinalIgnoreCase, "TXI", "TXS", "TXR", "TXA");

    private static readonly ImmutableDictionary<string, string> SetStateMap =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "CLOSE", SetStates.Closed },
            { "STARTUP", SetStates.Closed },
            { "MARKETCLOSE", SetStates.Closed },
            { "SAVECLOSING", SetStates.Closed },
            { "RESETTAT", SetStates.Closed },
            { "TRANSITION3", SetStates.Closed },
            { "PRE-OPEN", SetStates.PreOpen },
            { "PRE-OPEN1", SetStates.PreOpen1 },
            { "PRE-OPEN2", SetStates.PreOpen2 },
            { "OPEN", SetStates.Open },
            { "OPEN1", SetStates.Open1 },
            { "OPEN2", SetStates.Open2 },
            { "INTERMISSION", SetStates.Intermission },
            { "PRE-CLOSE", SetStates.PreClose },
            { "PRE-CLOSE1", SetStates.PreClose },
            { "OFF-HOUR", SetStates.OffHour },
            { "FREEZE1", SetStates.Freeze1 },
            { "FREEZE2", SetStates.Freeze2 },
            { "FREEZE3", SetStates.Freeze3 },
            { "PRE-DAY", SetStates.PreDay },
            { "DAY", SetStates.Day },
            { "INTERMISSION1", SetStates.IntermissionNight },
            { "INTERMISSION2", SetStates.IntermissionNight },
            { "PRE-NIGHT", SetStates.PreNight },
            { "NIGHT", SetStates.Night },
        }.ToImmutableDictionary();

    private static readonly ImmutableDictionary<string, string> TfexStateMap =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "CLOSE", TfexStates.Closed },
            { "SERIESENAY", TfexStates.Closed },
            { "COMBOENAY", TfexStates.Closed },
            { "DAYCLOSE", TfexStates.Closed },
            { "RESETTAT", TfexStates.Closed },
            { "TRANSITION3", TfexStates.Closed },
            { "MARKETCLOSE", TfexStates.Closed },
            { "PRE-MORNING", TfexStates.PreMorning },
            { "MORNING", TfexStates.MorningSession },
            { "DAY", TfexStates.DaySession },
            { "TRANSITION1", TfexStates.Intermission },
            { "INTERMISSION", TfexStates.Intermission },
            { "INTERMISSION2", TfexStates.IntermissionNight },
            { "INTERMISSION3", TfexStates.IntermissionNight },
            { "PRE-AFTERNOON", TfexStates.PreAfternoon },
            { "AFTERNOON", TfexStates.AfternoonSession },
            { "PRE-SETTLEMENT", TfexStates.IntermissionNight },
            { "SERIESENIGHT", TfexStates.IntermissionNight },
            { "PRE-NIGHT", TfexStates.PreNight },
            { "NIGHT", TfexStates.NightSession },
            // Add these two mappings to handle the special cases
            { "TRANSITION2", TfexStates.IntermissionNight },
            { "SETTLEMENT", TfexStates.IntermissionNight }
        }.ToImmutableDictionary();

    public static string MapStateName(string? stateName, string? marketSegment)
    {
        if (string.IsNullOrWhiteSpace(stateName))
            return string.Empty;

        var trimmedStateName = stateName.Trim().ToUpperInvariant();
        trimmedStateName = StateNameRegex.Replace(trimmedStateName, "");

        if (SetStateMap.TryGetValue(trimmedStateName, out var setMappedState))
            return setMappedState;

        if (TfexStateMap.TryGetValue(trimmedStateName, out var tfexMappedState))
        {
            if (trimmedStateName is "TRANSITION2" or "SETTLEMENT" &&
                MarketSegmentDayMarket.Contains(marketSegment ?? string.Empty))
                return TfexStates.Closed;

            return tfexMappedState;
        }

        return stateName;
    }

    // SET constants
    private static class SetStates
    {
        public const string Closed = "Closed";
        public const string PreOpen1 = "Pre-Open1";
        public const string PreOpen = "Pre-Open";
        public const string Open1 = "Open1";
        public const string Open = "Open";
        public const string Intermission = "Intermission";
        public const string PreOpen2 = "Pre-Open2";
        public const string Open2 = "Open2";
        public const string PreClose = "Pre-Close";
        public const string OffHour = "OffHour";
        public const string Freeze1 = "FREEZE1_E";
        public const string Freeze2 = "FREEZE2_E";
        public const string Freeze3 = "FREEZE3_E";
        public const string PreDay = "Pre-Day";
        public const string Day = "Day";
        public const string IntermissionNight = "Intermission-Night";
        public const string PreNight = "Pre-Night";
        public const string Night = "Night";
    }

    // TFEX constants
    private static class TfexStates
    {
        public const string Closed = "Closed";
        public const string PreMorning = "Pre-Morning";
        public const string MorningSession = "MorningSession";
        public const string DaySession = "DaySession";
        public const string Intermission = "Intermission";
        public const string PreAfternoon = "Pre-Afternoon";
        public const string AfternoonSession = "AfternoonSession";
        public const string IntermissionNight = "Intermission-Night";
        public const string PreNight = "Pre-Night";
        public const string NightSession = "NightSession";
    }
}
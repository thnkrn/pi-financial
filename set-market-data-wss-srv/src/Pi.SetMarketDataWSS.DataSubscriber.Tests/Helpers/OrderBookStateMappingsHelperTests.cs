using Pi.SetMarketDataWSS.Application.Helpers;

namespace Pi.SetMarketDataWSS.DataSubscriber.Tests.Helpers;

public class OrderBookStateMappingsHelperTests
{
    [Theory]
    // SET mappings
    [InlineData("CLOSE_E", null, "Closed")]
    [InlineData("STARTUP_E", null, "Closed")]
    [InlineData("MARKETCLOSE_E", null, "Closed")]
    [InlineData("SAVECLOSING_E", null, "Closed")]
    [InlineData("RESET_STAT_E", null, "Closed")]
    [InlineData("CLOSE_L", null, "Closed")]
    [InlineData("MARKETCLOSE_L", null, "Closed")]
    [InlineData("PRE-OPEN1_E", null, "Pre-Open1")]
    [InlineData("PRE-OPEN_E", null, "Pre-Open")]
    [InlineData("PRE-OPEN_L", null, "Pre-Open")]
    [InlineData("OPEN1_E", null, "Open1")]
    [InlineData("OPEN_E", null, "Open")]
    [InlineData("INTERMISSION_E", null, "Intermission")]
    [InlineData("PRE-OPEN2_E", null, "Pre-Open2")]
    [InlineData("OPEN2_E", null, "Open2")]
    [InlineData("PRE-CLOSE_E", null, "Pre-Close")]
    [InlineData("OFF-HOUR_E", null, "OffHour")]
    [InlineData("OFF-HOUR_L", null, "OffHour")]
    [InlineData("FREEZE1_E", null, "FREEZE1_E")]
    [InlineData("FREEZE2_E", null, "FREEZE2_E")]
    [InlineData("FREEZE3_E", null, "FREEZE3_E")]
    // TFEX mappings
    [InlineData("CLOSE_D", null, "Closed")]
    [InlineData("SERIES_GEN_DAY_D", null, "Closed")]
    [InlineData("COMBO_GEN_DAY_D", null, "Closed")]
    [InlineData("TRANSITION2_D", "TXI", "Closed")]
    [InlineData("TRANSITION2_D", "TXS", "Closed")]
    [InlineData("TRANSITION2_D", "TXR", "Closed")]
    [InlineData("TRANSITION2_D", "TXA", "Closed")]
    [InlineData("TRANSITION2_D", "OTHER", "Intermission-Night")]
    [InlineData("SETTLEMENT_D", "TXI", "Closed")]
    [InlineData("SETTLEMENT_D", "TXS", "Closed")]
    [InlineData("SETTLEMENT_D", "TXR", "Closed")]
    [InlineData("SETTLEMENT_D", "TXA", "Closed")]
    [InlineData("SETTLEMENT_D", "OTHER", "Intermission-Night")]
    [InlineData("DAYCLOSE_D", null, "Closed")]
    [InlineData("RESET_STAT_D", null, "Closed")]
    [InlineData("TRANSITION3_D", null, "Closed")]
    [InlineData("MARKETCLOSE_D", null, "Closed")]
    [InlineData("PRE-MORNING_D", null, "Pre-Morning")]
    [InlineData("MORNING_D", null, "MorningSession")]
    [InlineData("DAY_D", null, "DaySession")]
    [InlineData("TRANSITION1_D", null, "Intermission")]
    [InlineData("INTERMISSION_D", null, "Intermission")]
    [InlineData("PRE-AFTERNOON_D", null, "Pre-Afternoon")]
    [InlineData("AFTERNOON_D", null, "AfternoonSession")]
    [InlineData("PRE-SETTLEMENT_D", null, "Intermission-Night")]
    [InlineData("INTERMISSION2_D", null, "Intermission-Night")]
    [InlineData("SERIES_GEN_NIGHT_D", null, "Intermission-Night")]
    [InlineData("INTERMISSION3_D", null, "Intermission-Night")]
    [InlineData("PRE-NIGHT_D", null, "Pre-Night")]
    [InlineData("NIGHT_D", null, "NightSession")]
    // Edge cases
    [InlineData("UNKNOWN_STATE", null, "")]
    [InlineData(null, null, "")]
    public void MapStateName_ShouldReturnExpectedResult(string input, string marketSegment, string expected)
    {
        // Act
        var result = OrderBookStateMappingsHelper.MapStateName(input, marketSegment);

        // Assert
        Assert.Equal(expected, result);
    }
}
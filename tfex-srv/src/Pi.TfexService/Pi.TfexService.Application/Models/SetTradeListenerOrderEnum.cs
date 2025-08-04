using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Pi.TfexService.Application.Models;

public static class SetTradeListenerOrderEnum
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Side
    {
        [EnumMember(Value = "UndefinedLongShort")]
        UndefinedLongShort = 0,
        [EnumMember(Value = "Long")] Long = 1, // Buy
        [EnumMember(Value = "Short")] Short = 2, // Sell
        [EnumMember(Value = "LongAndShort")] LongAndShort = 3,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Position
    {
        [EnumMember(Value = "UndefinedPosition")]
        UndefinedPosition = 0,
        [EnumMember(Value = "Open")] Open = 1, // Open Position
        [EnumMember(Value = "Close")] Close = 2, // Close Position
        [EnumMember(Value = "Auto")] Auto = 3, // Auto Position (extra permission required)
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PriceType
    {
        [EnumMember(Value = "UndefinedPriceType")]
        UndefinedPriceType = 0,
        [EnumMember(Value = "Limit")] Limit = 1, // Limit Order
        [EnumMember(Value = "ATO")] Ato = 2, // At The Open, field price must be 0
        [EnumMember(Value = "ATC")] Atc = 3, // At The Close
        [EnumMember(Value = "MP-MKT")] MpMkt = 4, // Market To Limit Order, field price must be 0
        [EnumMember(Value = "MP-MTL")] MpMtl = 6, // Market Order, field price must be 0
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Validity
    {
        [EnumMember(Value = "UndefinedValidity")]
        UndefinedValidity = 0,
        [EnumMember(Value = "FOK")] Fok = 1, // Fill or Kill
        [EnumMember(Value = "IOC")] Ioc = 2, // Immediate or Cancel
        [EnumMember(Value = "Date")] Date = 3, // GTD order will be available to specific date
        [EnumMember(Value = "Cancel")] Cancel = 4, // GTC order will be available maximum 254 days after business date
        [EnumMember(Value = "Day")] Day = 8, // Order will be available only in order entry date
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TriggerCondition
    {
        [EnumMember(Value = "ASK_OR_HIGHER"), Group("Price Movement")]
        AskOrHigher = 1, // Stop price >= Ask price (Condition type: Price Movement)

        [EnumMember(Value = "ASK_OR_LOWER"), Group("Price Movement")]
        AskOrLower = 2, // Stop price <= Ask price (Condition type: Price Movement)

        [EnumMember(Value = "BID_OR_HIGHER"), Group("Price Movement")]
        BidOrHigher = 3, // Stop price >= Bid price (Condition type: Price Movement)

        [EnumMember(Value = "BID_OR_LOWER"), Group("Price Movement")]
        BidOrLower = 4, // Stop price <= Bid price (Condition type: Price Movement)

        [EnumMember(Value = "LAST_PAID_OR_HIGHER"), Group("Price Movement")]
        LastPaidOrHigher = 5, // Stop price >= Last price (Condition type: Price Movement)

        [EnumMember(Value = "LAST_PAID_OR_LOWER"), Group("Price Movement")]
        LastPaidOrLower = 6, // Stop price <= Last price (Condition type: Price Movement)

        [EnumMember(Value = "SESSION"), Group("Session")]
        Session = 7, // Stop order will trigger when trading session change (Condition type: Session Change)
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TriggerSession
    {
        [EnumMember(Value = "Pre-Open1")] PreOpen1 = 1, // Pre Open before Morning Session and Day Session
        [EnumMember(Value = "Open1")] Open1 = 2, // Morning Session
        [EnumMember(Value = "Day")] Day = 3, // Day Session (for non-intermission product e.g. RSS, RSS3D)
        [EnumMember(Value = "Pre-Open2")] PreOpen2 = 4, // Pre Open before Afternoon Session
        [EnumMember(Value = "Open2")] Open2 = 5, // Afternoon Session

        [EnumMember(Value = "Pre-Open0")]
        PreOpen0 = 6, // Pre Open before Night Session (for night session product e.g. GOLD, CURRENCY)
        [EnumMember(Value = "Open0")] Open0 = 7, // Night Session (for night session product e.g. GOLD, CURRENCY)
    }

    public class GroupAttribute(string groupName) : Attribute
    {
        public string GroupName { get; } = groupName;
    }
}
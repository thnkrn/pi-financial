using System.Runtime.Serialization;
using Microsoft.OpenApi.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SetTradeClient = Pi.Financial.Client.SetTradeOms.Model;

namespace Pi.TfexService.Application.Models;

[JsonConverter(typeof(StringEnumConverter))]
public enum Side
{
    [EnumMember(Value = "Unknown")] Unknown = 0, // Unknown
    [EnumMember(Value = "Long")] Long = 1, // Buy
    [EnumMember(Value = "Short")] Short = 2, // Sell
}

[JsonConverter(typeof(StringEnumConverter))]
public enum Position
{
    [EnumMember(Value = "Unknown")] Unknown = 0, // Unknown
    [EnumMember(Value = "Auto")] Auto = 1, // Auto Position (extra permission required)
    [EnumMember(Value = "Open")] Open = 2, // Open Position
    [EnumMember(Value = "Close")] Close = 3, // Close Position
}

[JsonConverter(typeof(StringEnumConverter))]
public enum PriceType
{
    [EnumMember(Value = "Limit")][Display("Limit")] Limit = 1, // Limit Order
    [EnumMember(Value = "ATO")][Display("ATO")] Ato = 2, // At The Open, field price must be 0
    [EnumMember(Value = "MP-MTL")][Display("MP-MTL")] MpMtl = 3, // Market To Limit Order, field price must be 0
    [EnumMember(Value = "MP-MKT")][Display("MP-MKT")] MpMkt = 4, // Market Order, field price must be 0
}

[JsonConverter(typeof(StringEnumConverter))]
public enum Validity
{
    [EnumMember(Value = "Day")] Day = 1, // Order will be available only in order entry date
    [EnumMember(Value = "FOK")] Fok = 2, // Fill or Kill
    [EnumMember(Value = "IOC")] Ioc = 3, // Immediate or Cancel
    [EnumMember(Value = "Date")] Date = 4, // GTD order will be available to specific date
    [EnumMember(Value = "Cancel")] Cancel = 5, // GTC order will be available maximum 254 days after business date
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
    [EnumMember(Value = "Pre-Open1")]
    PreOpen1 = 1, // Pre Open before Morning Session and Day Session
    [EnumMember(Value = "Open1")]
    Open1 = 2, // Morning Session
    [EnumMember(Value = "Day")]
    Day = 3, // Day Session (for non-intermission product e.g. RSS, RSS3D)
    [EnumMember(Value = "Pre-Open2")]
    PreOpen2 = 4, // Pre Open before Afternoon Session
    [EnumMember(Value = "Open2")]
    Open2 = 5, // Afternoon Session
    [EnumMember(Value = "Pre-Open0")]
    PreOpen0 = 6, // Pre Open before Night Session (for night session product e.g. GOLD, CURRENCY)
    [EnumMember(Value = "Open0")]
    Open0 = 7, // Night Session (for night session product e.g. GOLD, CURRENCY)
}

public class GroupAttribute(string groupName) : Attribute
{
    public string GroupName { get; } = groupName;
}

public static class SetTradeEnumExtension
{
    public static Side ToSide(this SetTradeClient.Side side)
    {
        return side switch
        {
            SetTradeClient.Side.Long => Side.Long,
            SetTradeClient.Side.Short => Side.Short,
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, $"Not expected Side value: {side}")
        };
    }

    public static SetTradeClient.Side ToSide(this Side side)
    {
        return side switch
        {
            Side.Long => SetTradeClient.Side.Long,
            Side.Short => SetTradeClient.Side.Short,
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, $"Not expected Side value: {side}")
        };
    }

    public static Position ToPosition(this SetTradeClient.Position position)
    {
        return position switch
        {
            SetTradeClient.Position.Auto => Position.Auto,
            SetTradeClient.Position.Open => Position.Open,
            SetTradeClient.Position.Close => Position.Close,
            _ => throw new ArgumentOutOfRangeException(nameof(position), position, $"Not expected Position value: {position}")
        };
    }

    public static SetTradeClient.Position ToPosition(this Position position)
    {
        return position switch
        {
            Position.Auto => SetTradeClient.Position.Auto,
            Position.Open => SetTradeClient.Position.Open,
            Position.Close => SetTradeClient.Position.Close,
            _ => throw new ArgumentOutOfRangeException(nameof(position), position, $"Not expected Position value: {position}")
        };
    }

    public static PriceType ToPriceType(this SetTradeClient.PriceType priceType)
    {
        return priceType switch
        {
            SetTradeClient.PriceType.Limit => PriceType.Limit,
            SetTradeClient.PriceType.ATO => PriceType.Ato,
            SetTradeClient.PriceType.MPMTL => PriceType.MpMtl,
            SetTradeClient.PriceType.MPMKT => PriceType.MpMkt,
            _ => throw new ArgumentOutOfRangeException(nameof(priceType), priceType, $"Not expected PriceType value: {priceType}")
        };
    }

    public static SetTradeClient.PriceType ToPriceType(this PriceType priceType)
    {
        return priceType switch
        {
            PriceType.Limit => SetTradeClient.PriceType.Limit,
            PriceType.Ato => SetTradeClient.PriceType.ATO,
            PriceType.MpMtl => SetTradeClient.PriceType.MPMTL,
            PriceType.MpMkt => SetTradeClient.PriceType.MPMKT,
            _ => throw new ArgumentOutOfRangeException(nameof(priceType), priceType, $"Not expected PriceType value: {priceType}")
        };
    }

    public static Validity ToValidityType(this SetTradeClient.ValidityType validity)
    {
        return validity switch
        {
            SetTradeClient.ValidityType.Day => Validity.Day,
            SetTradeClient.ValidityType.FOK => Validity.Fok,
            SetTradeClient.ValidityType.IOC => Validity.Ioc,
            SetTradeClient.ValidityType.Date => Validity.Date,
            SetTradeClient.ValidityType.Cancel => Validity.Cancel,
            _ => throw new ArgumentOutOfRangeException(nameof(validity), validity, $"Not expected ValidityType value: {validity}")
        };
    }

    public static SetTradeClient.ValidityType ToValidityType(this Validity validity)
    {
        return validity switch
        {
            Validity.Day => SetTradeClient.ValidityType.Day,
            Validity.Fok => SetTradeClient.ValidityType.FOK,
            Validity.Ioc => SetTradeClient.ValidityType.IOC,
            Validity.Date => SetTradeClient.ValidityType.Date,
            Validity.Cancel => SetTradeClient.ValidityType.Cancel,
            _ => throw new ArgumentOutOfRangeException(nameof(validity), validity, $"Not expected Validity value: {validity}")
        };
    }

    public static TriggerCondition ToTriggerCondition(this SetTradeClient.TriggerCondition triggerCondition)
    {
        return triggerCondition switch
        {
            SetTradeClient.TriggerCondition.ASKORHIGHER => TriggerCondition.AskOrHigher,
            SetTradeClient.TriggerCondition.ASKORLOWER => TriggerCondition.AskOrLower,
            SetTradeClient.TriggerCondition.BIDORHIGHER => TriggerCondition.BidOrHigher,
            SetTradeClient.TriggerCondition.BIDORLOWER => TriggerCondition.BidOrLower,
            SetTradeClient.TriggerCondition.LASTPAIDORHIGHER => TriggerCondition.LastPaidOrHigher,
            SetTradeClient.TriggerCondition.LASTPAIDORLOWER => TriggerCondition.LastPaidOrLower,
            SetTradeClient.TriggerCondition.SESSION => TriggerCondition.Session,
            _ => throw new ArgumentOutOfRangeException(nameof(triggerCondition), triggerCondition, $"Not expected TriggerCondition value: {triggerCondition}")
        };
    }

    public static SetTradeClient.TriggerCondition ToTriggerCondition(this TriggerCondition triggerCondition)
    {
        return triggerCondition switch
        {
            TriggerCondition.AskOrHigher => SetTradeClient.TriggerCondition.ASKORHIGHER,
            TriggerCondition.AskOrLower => SetTradeClient.TriggerCondition.ASKORLOWER,
            TriggerCondition.BidOrHigher => SetTradeClient.TriggerCondition.BIDORHIGHER,
            TriggerCondition.BidOrLower => SetTradeClient.TriggerCondition.BIDORLOWER,
            TriggerCondition.LastPaidOrHigher => SetTradeClient.TriggerCondition.LASTPAIDORHIGHER,
            TriggerCondition.LastPaidOrLower => SetTradeClient.TriggerCondition.LASTPAIDORLOWER,
            TriggerCondition.Session => SetTradeClient.TriggerCondition.SESSION,
            _ => throw new ArgumentOutOfRangeException(nameof(triggerCondition), triggerCondition, $"Not expected TriggerCondition value: {triggerCondition}")
        };
    }

    public static TriggerSession ToTriggerSession(this SetTradeClient.TriggerSession triggerSession)
    {
        return triggerSession switch
        {
            SetTradeClient.TriggerSession.PreOpen1 => TriggerSession.PreOpen1,
            SetTradeClient.TriggerSession.Open1 => TriggerSession.Open1,
            SetTradeClient.TriggerSession.Day => TriggerSession.Day,
            SetTradeClient.TriggerSession.PreOpen2 => TriggerSession.PreOpen2,
            SetTradeClient.TriggerSession.Open2 => TriggerSession.Open2,
            SetTradeClient.TriggerSession.PreOpen0 => TriggerSession.PreOpen0,
            SetTradeClient.TriggerSession.Open0 => TriggerSession.Open0,
            _ => throw new ArgumentOutOfRangeException(nameof(triggerSession), triggerSession, $"Not expected TriggerSession value: {triggerSession}")
        };
    }

    public static SetTradeClient.TriggerSession ToTriggerSession(this TriggerSession triggerSession)
    {
        return triggerSession switch
        {
            TriggerSession.PreOpen1 => SetTradeClient.TriggerSession.PreOpen1,
            TriggerSession.Open1 => SetTradeClient.TriggerSession.Open1,
            TriggerSession.Day => SetTradeClient.TriggerSession.Day,
            TriggerSession.PreOpen2 => SetTradeClient.TriggerSession.PreOpen2,
            TriggerSession.Open2 => SetTradeClient.TriggerSession.Open2,
            TriggerSession.PreOpen0 => SetTradeClient.TriggerSession.PreOpen0,
            TriggerSession.Open0 => SetTradeClient.TriggerSession.Open0,
            _ => throw new ArgumentOutOfRangeException(nameof(triggerSession), triggerSession, $"Not expected TriggerSession value: {triggerSession}")
        };
    }
}
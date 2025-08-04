using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Pi.TfexService.Application.Models;

[JsonConverter(typeof(StringEnumConverter))]
public enum InstrumentCategory
{
    Unstructure, // todo: remove once move to AppSynth
    Unstructured,
    Others
}

[JsonConverter(typeof(StringEnumConverter))]
public enum MultiplierCategory
{
    Set50IndexFutures,
    Set50IndexOptions,
    SingleStockFutures,
    SectorIndexFutures
}

[JsonConverter(typeof(StringEnumConverter))]
public enum SectorCategory
{
    Bank,
    Ict,
    Energ,
    Comm,
    Food
}

[JsonConverter(typeof(StringEnumConverter))]
public enum TickSizeCategory
{
    Set50IndexFutures,
    Set50IndexOptions,
    SingleStockFutures,
    SectorIndexFutures,
    PreciousMetalFutures,
    CurrencyFutures,
    Others
}

[JsonConverter(typeof(StringEnumConverter))]
public enum PreciousMetalCategory
{
    Gf,
    Gf10,
    Go,
    Svf,
    Gd
}

[JsonConverter(typeof(StringEnumConverter))]
public enum CurrencyCategory
{
    Usd,
    Eur,
    Jpy,
    EurUsd,
    UsdJpy
}

[JsonConverter(typeof(StringEnumConverter))]
public enum OtherCategory
{
    Rss3,
    Rss3d,
    Jrf
}

[JsonConverter(typeof(StringEnumConverter))]
public enum MultiplierType
{
    ContractLot,
    Multiplier
}

[JsonConverter(typeof(StringEnumConverter))]
public enum MultiplierUnit
{
    [EnumMember(Value = "THB / point")] Point,
    [EnumMember(Value = "shares")] Shares,
}

[JsonConverter(typeof(StringEnumConverter))]
public enum Venue
{
    Derivative
}
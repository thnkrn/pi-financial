using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Application.Services.Models.ItchParser;

namespace Pi.SetMarketData.Application.Models.ItchMessageWrapper;

// ReSharper disable InconsistentNaming
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
public class OrderBookDirectoryMessageWrapper : ItchMessage
{
#pragma warning disable CS8618, CS9264
    public OrderBookDirectoryMessageWrapper()
#pragma warning restore CS8618, CS9264
    {
        MsgType = ItchMessageType.R;
    }

    public Nanos Nanos { get; set; }
    public OrderBookID OrderBookID { get; set; }
    public Symbol Symbol { get; set; }
    public LongName LongName { get; set; }
    public ISIN ISIN { get; set; }
    public FinancialProduct FinancialProduct { get; set; }
    public TradingCurrency TradingCurrency { get; set; }
    public DecimalsInPrice DecimalsInPrice { get; set; }
    public DecimalsInNominalValue DecimalsInNominalValue { get; set; }
    public RoundLotSize RoundLotSize { get; set; }
    public NominalValue NominalValue { get; set; }
    public NumberOfLegs NumberOfLegs { get; set; }
    public UnderlyingName UnderlyingName { get; set; }
    public Underlying Underlying { get; set; }
    public UnderlyingOrderBookID UnderlyingOrderBookID { get; set; }
    public StrikePrice StrikePrice { get; set; }
    public ExpirationDate ExpirationDate { get; set; }
    public DecimalsInStrikePrice DecimalsInStrikePrice { get; set; }
    public OptionType OptionType { get; set; }
    public ExchangeCode ExchangeCode { get; set; }
    public MarketCode MarketCode { get; set; }
    public PriceQuotationFactor PriceQuotationFactor { get; set; }
    public CorporateActionCode CorporateActionCode { get; set; }
    public NotificationSign NotificationSign { get; set; }
    public OtherSign OtherSign { get; set; }
    public AllowNvdr AllowNvdr { get; set; }
    public AllowShortSell AllowShortSell { get; set; }
    public AllowShortSellOnNvdr AllowShortSellOnNvdr { get; set; }
    public AllowTtf AllowTtf { get; set; }
    public ParValue ParValue { get; set; }
    public FirstTradingDate FirstTradingDate { get; set; }
    public FirstTradingTime FirstTradingTime { get; set; }
    public LastTradingDate LastTradingDate { get; set; }
    public LastTradingTime LastTradingTime { get; set; }
    public MarketSegment MarketSegment { get; set; }
    public PhysicalDelivery PhysicalDelivery { get; set; }
    public ContractSize ContractSize { get; set; }
    public SectorCode SectorCode { get; set; }
    public OriginatesFrom OriginatesFrom { get; set; }
    public Status Status { get; set; }
    public Modifier Modifier { get; set; }
    public NotationDate NotationDate { get; set; }
    public DecimalsInContractSizePQF DecimalsInContractSizePQF { get; set; }
    public Metadata Metadata { get; set; }

    public new char MsgType
    {
        get => base.MsgType;
        private set => base.MsgType = value;
    }
}

public class AllowNvdr
{
    public string? Value { get; set; }
}

public class AllowShortSell
{
    public string? Value { get; set; }
}

public class AllowShortSellOnNvdr
{
    public string? Value { get; set; }
}

public class AllowTtf
{
    public string? Value { get; set; }
}

public class ContractSize
{
    public int Value { get; set; }
}

public class CorporateActionCode
{
    public string? Value { get; set; }
}

public class DecimalsInContractSizePQF
{
    public int Value { get; set; }
}

public class DecimalsInNominalValue
{
    public int Value { get; set; }
}

public class DecimalsInPrice
{
    public int Value { get; set; }
}

public class DecimalsInStrikePrice
{
    public int Value { get; set; }
}

public class ExchangeCode
{
    public int Value { get; set; }
}

public class ExpirationDate
{
}

public class FinancialProduct
{
    public string? Value { get; set; }
}

public class FirstTradingDate
{
}

public class FirstTradingTime
{
    public int Hours { get; set; }
    public int Minutes { get; set; }
    public int Seconds { get; set; }
}

public class ISIN
{
    public string? Value { get; set; }
}

public class LastTradingDate
{
    public DateTime? Value { get; set; }
}

public class LastTradingTime
{
    public int Hours { get; set; }
    public int Minutes { get; set; }
    public int Seconds { get; set; }
}

public class LongName
{
    public string? Value { get; set; }
}

public class MarketCode
{
    public int Value { get; set; }
}

public class MarketSegment
{
    public string? Value { get; set; }
}

public class Modifier
{
    public int Value { get; set; }
}

public class Metadata
{
    public long Timestamp { get; set; }
    public string? Session { get; set; }
    public ulong SequenceNumber { get; set; }
}

public class Nanos
{
    public int Value { get; set; }
}

public class NominalValue
{
    public int Value { get; set; }
}

public class NotationDate
{
}

public class NotificationSign
{
    public string? Value { get; set; }
}

public class NumberOfLegs
{
    public int Value { get; set; }
}

public class OptionType
{
    public int Value { get; set; }
}

public class OrderBookID
{
    public int Value { get; set; }
}

public class OriginatesFrom
{
    public string? Value { get; set; }
}

public class OtherSign
{
    public string? Value { get; set; }
}

public class ParValue
{
    public int Value { get; set; }
}

public class PhysicalDelivery
{
    public string? Value { get; set; }
}

public class PriceQuotationFactor
{
    public long Value { get; set; }
    public int NumberOfDecimals { get; set; }
}

public class RoundLotSize
{
    public int Value { get; set; }
}

public class SectorCode
{
    public string? Value { get; set; }
}

public class Status
{
    public string? Value { get; set; }
}

public class StrikePrice
{
    public long Value { get; set; }
    public int NumberOfDecimals { get; set; }
}

public class Symbol
{
    public string? Value { get; set; }
}

public class TradingCurrency
{
    public string? Value { get; set; }
}

public class Underlying
{
    public int Value { get; set; }
}

public class UnderlyingName
{
    public string? Value { get; set; }
}

public class UnderlyingOrderBookID
{
    public int? Value { get; set; }
}
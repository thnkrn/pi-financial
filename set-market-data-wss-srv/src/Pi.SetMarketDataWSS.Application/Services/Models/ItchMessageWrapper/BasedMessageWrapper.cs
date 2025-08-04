namespace Pi.SetMarketDataWSS.Application.Services.Models.ItchMessageWrapper;

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
    public string? Value { get; set; }
}

public class FinancialProduct
{
    public string? Value { get; set; }
}

public class FirstTradingDate
{
    public string? Value { get; set; }
}

public class FirstTradingTime
{
    public int Hours { get; set; }
    public int Minutes { get; set; }
    public int Seconds { get; set; }
}

public class LastTradingDate
{
    public string? Value { get; set; }
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
    public string? Value { get; set; }
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

public class DecimalsInContractSizePqf
{
    public int Value { get; set; }
}

public class Isin
{
    public string? Value { get; set; }
}

public class Metadata
{
    public long Timestamp { get; set; }
    public string? Session { get; set; }
    public ulong SequenceNumber { get; set; }
    public string? OrderBookId { get; set; }
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
    public long Value { get; set; }
}

public class UnderlyingName
{
    public string? Value { get; set; }
}

public class UnderlyingOrderBookId
{
    public int Value { get; set; }
}

public class OrderBookId
{
    public int Value { get; set; }
}

public class StateName
{
    public string? Value { get; set; }
}

public class AveragePrice
{
    public long Value { get; set; }
    public int NumberOfDecimals { get; set; }
}

public class HighPrice
{
    public long Value { get; set; }
    public int NumberOfDecimals { get; set; }
}

public class LastAuctionPrice
{
    public long Value { get; set; }
    public int NumberOfDecimals { get; set; }
}

public class LastPrice
{
    public long Value { get; set; }
    public int NumberOfDecimals { get; set; }
}

public class LowPrice
{
    public long Value { get; set; }
    public int NumberOfDecimals { get; set; }
}

public class OpenPrice
{
    public long Value { get; set; }
    public int NumberOfDecimals { get; set; }
}

public class ReportedTurnoverQuantity
{
    public long Value { get; set; }
}

public class ReportedTurnoverValue
{
    public long Value { get; set; }
    public int NumberOfDecimals { get; set; }
}

public class TotalNumberOfTrades
{
    public int Value { get; set; }
}

public class TurnoverQuantity
{
    public int Value { get; set; }
}

public class TurnoverValue
{
    public long Value { get; set; }
    public int NumberOfDecimals { get; set; }
}

public class Action
{
    public int Value { get; set; }
}

public class DealId
{
    public long Value { get; set; }
}

public class DealSource
{
    public int Value { get; set; }
}

public class TradeReportCode
{
    public int Value { get; set; }
}

public class UpperLimit
{
    public long Value { get; set; }
    public int NumberOfDecimals { get; set; }
}

public class LowerLimit
{
    public long Value { get; set; }
    public int NumberOfDecimals { get; set; }
}

public class MarketDescription
{
    public string? Value { get; set; }
}

public class MarketName
{
    public string? Value { get; set; }
}

public class BidQuantity
{
    public int? Value { get; set; }
}

public class AskQuantity
{
    public int? Value { get; set; }
}

public class EquilibriumPrice
{
    public long? Value { get; set; }
}

public class NumberOfDecimals
{
    public int? Value { get; set; }
}

public class Price
{
    public long Value { get; set; }
    public int NumberOfDecimals { get; set; }
}

public class Quantity
{
    public long Value { get; set; }
}
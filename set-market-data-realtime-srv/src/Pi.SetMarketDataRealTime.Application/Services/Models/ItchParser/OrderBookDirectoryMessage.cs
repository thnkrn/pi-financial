using Pi.SetMarketDataRealTime.Application.Services.Constants;
using Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;
using Pi.SetMarketDataRealTime.Application.Utils;

namespace Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;

public class OrderBookDirectoryParams
{
    public required Numeric32 Nanos { get; init; }
    public required Numeric32 OrderBookID { get; init; }
    public Alpha? Symbol { get; init; }
    public Alpha? LongName { get; init; }
    public Alpha? Isin { get; init; }
    public Alpha? FinancialProduct { get; init; }
    public Alpha? TradingCurrency { get; init; }
    public required Numeric16 DecimalsInPrice { get; init; }
    public required Numeric16 DecimalsInNominalValue { get; init; }
    public required Numeric32 RoundLotSize { get; init; }
    public required Numeric64 NominalValue { get; init; }
    public required Numeric8 NumberOfLegs { get; init; }
    public Alpha? UnderlyingName { get; init; }
    public required Numeric32 Underlying { get; init; }
    public required Numeric32 UnderlyingOrderBookID { get; init; }
    public required Price32 StrikePrice { get; init; }
    public required Date ExpirationDate { get; init; }
    public required Numeric16 DecimalsInStrikePrice { get; init; }
    public required Numeric8 OptionType { get; init; }
    public required Numeric8 ExchangeCode { get; init; }
    public required Numeric8 MarketCode { get; init; }
    public required Price64 PriceQuotationFactor { get; init; }
    public Alpha? CorporateActionCode { get; init; }
    public Alpha? NotificationSign { get; init; }
    public Alpha? OtherSign { get; init; }
    public Alpha? AllowNvdr { get; init; }
    public Alpha? AllowShortSell { get; init; }
    public Alpha? AllowShortSellOnNvdr { get; init; }
    public Alpha? AllowTtf { get; init; }
    public required Numeric64 ParValue { get; init; }
    public required Date FirstTradingDate { get; init; }
    public required Time FirstTradingTime { get; init; }
    public required Date LastTradingDate { get; init; }
    public required Time LastTradingTime { get; init; }
    public Alpha? MarketSegment { get; init; }
    public Alpha? PhysicalDelivery { get; init; }
    public required Numeric32 ContractSize { get; init; }
    public Alpha? SectorCode { get; init; }
    public Alpha? OriginatesFrom { get; init; }
    public Alpha? Status { get; init; }
    public required Numeric16 Modifier { get; init; }
    public required Date NotationDate { get; init; }
    public required Numeric16 DecimalsInContractSizePQF { get; init; }
}

public class OrderBookDirectoryMessage : ItchMessage
{
    public OrderBookDirectoryMessage(OrderBookDirectoryParams orderBookDirectoryParams)
    {
        MsgType = ItchMessageType.R;
        Nanos = orderBookDirectoryParams.Nanos;
        OrderBookID = orderBookDirectoryParams.OrderBookID;
        Symbol = orderBookDirectoryParams.Symbol;
        LongName = orderBookDirectoryParams.LongName;
        ISIN = orderBookDirectoryParams.Isin;
        FinancialProduct = orderBookDirectoryParams.FinancialProduct;
        TradingCurrency = orderBookDirectoryParams.TradingCurrency;
        DecimalsInPrice = orderBookDirectoryParams.DecimalsInPrice;
        DecimalsInNominalValue = orderBookDirectoryParams.DecimalsInNominalValue;
        RoundLotSize = orderBookDirectoryParams.RoundLotSize;
        NominalValue = orderBookDirectoryParams.NominalValue;
        NumberOfLegs = orderBookDirectoryParams.NumberOfLegs;
        UnderlyingName = orderBookDirectoryParams.UnderlyingName;
        Underlying = orderBookDirectoryParams.Underlying;
        UnderlyingOrderBookID = orderBookDirectoryParams.UnderlyingOrderBookID;
        StrikePrice = orderBookDirectoryParams.StrikePrice;
        ExpirationDate = orderBookDirectoryParams.ExpirationDate;
        DecimalsInStrikePrice = orderBookDirectoryParams.DecimalsInStrikePrice;
        OptionType = orderBookDirectoryParams.OptionType;
        ExchangeCode = orderBookDirectoryParams.ExchangeCode;
        MarketCode = orderBookDirectoryParams.MarketCode;
        PriceQuotationFactor = orderBookDirectoryParams.PriceQuotationFactor;
        CorporateActionCode = orderBookDirectoryParams.CorporateActionCode;
        NotificationSign = orderBookDirectoryParams.NotificationSign;
        OtherSign = orderBookDirectoryParams.OtherSign;
        AllowNvdr = orderBookDirectoryParams.AllowNvdr;
        AllowShortSell = orderBookDirectoryParams.AllowShortSell;
        AllowShortSellOnNvdr = orderBookDirectoryParams.AllowShortSellOnNvdr;
        AllowTtf = orderBookDirectoryParams.AllowTtf;
        ParValue = orderBookDirectoryParams.ParValue;
        FirstTradingDate = orderBookDirectoryParams.FirstTradingDate;
        FirstTradingTime = orderBookDirectoryParams.FirstTradingTime;
        LastTradingDate = orderBookDirectoryParams.LastTradingDate;
        LastTradingTime = orderBookDirectoryParams.LastTradingTime;
        MarketSegment = orderBookDirectoryParams.MarketSegment;
        PhysicalDelivery = orderBookDirectoryParams.PhysicalDelivery;
        ContractSize = orderBookDirectoryParams.ContractSize;
        SectorCode = orderBookDirectoryParams.SectorCode;
        OriginatesFrom = orderBookDirectoryParams.OriginatesFrom;
        Status = orderBookDirectoryParams.Status;
        Modifier = orderBookDirectoryParams.Modifier;
        NotationDate = orderBookDirectoryParams.NotationDate;
        DecimalsInContractSizePQF = orderBookDirectoryParams.DecimalsInContractSizePQF;
    }

    public override Numeric32 Nanos { get; }
    public Numeric32 OrderBookID { get; }
    public Alpha? Symbol { get; }
    public Alpha? LongName { get; }
    public Alpha? ISIN { get; }
    public Alpha? FinancialProduct { get; }
    public Alpha? TradingCurrency { get; }
    public Numeric16 DecimalsInPrice { get; }
    public Numeric16 DecimalsInNominalValue { get; }
    public Numeric32 RoundLotSize { get; }
    public Numeric64 NominalValue { get; }
    public Numeric8 NumberOfLegs { get; }
    public Alpha? UnderlyingName { get; }
    public Numeric32 Underlying { get; }
    public Numeric32 UnderlyingOrderBookID { get; }
    public Price32 StrikePrice { get; }
    public Date ExpirationDate { get; }
    public Numeric16 DecimalsInStrikePrice { get; }
    public Numeric8 OptionType { get; }
    public Numeric8 ExchangeCode { get; }
    public Numeric8 MarketCode { get; }
    public Price64 PriceQuotationFactor { get; }
    public Alpha? CorporateActionCode { get; }
    public Alpha? NotificationSign { get; }
    public Alpha? OtherSign { get; }
    public Alpha? AllowNvdr { get; }
    public Alpha? AllowShortSell { get; }
    public Alpha? AllowShortSellOnNvdr { get; }
    public Alpha? AllowTtf { get; }
    public Numeric64 ParValue { get; }
    public Date FirstTradingDate { get; }
    public Time FirstTradingTime { get; }
    public Date LastTradingDate { get; }
    public Time LastTradingTime { get; }
    public Alpha? MarketSegment { get; }
    public Alpha? PhysicalDelivery { get; }
    public Numeric32 ContractSize { get; }
    public Alpha? SectorCode { get; }
    public Alpha? OriginatesFrom { get; }
    public Alpha? Status { get; }
    public Numeric16 Modifier { get; }
    public Date NotationDate { get; }
    public Numeric16 DecimalsInContractSizePQF { get; }

    public static OrderBookDirectoryMessage Parse(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        if (bytes.Length < 320)
            throw new ArgumentException("Invalid data format for OrderBookDirectoryMessage.", nameof(bytes));

        using var reader = new ItchMessageByteReader(bytes);
        var nanos = reader.ReadNumeric32();
        var orderBookId = reader.ReadNumeric32();
        var symbol = reader.ReadAlpha(32);
        var longName = reader.ReadAlpha(32);
        var isin = reader.ReadAlpha(12);
        var financialProduct = reader.ReadAlpha(3);
        var tradingCurrency = reader.ReadAlpha(3);
        var decimalsInPrice = reader.ReadNumeric16();
        var decimalsInNominalValue = reader.ReadNumeric16();
        var roundLotSize = reader.ReadNumeric32();
        var nominalValue = reader.ReadNumeric64();
        var numberOfLegs = reader.ReadNumeric8();
        var underlyingName = reader.ReadAlpha(6);
        var underlying = reader.ReadNumeric32();
        var underlyingOrderBookId = reader.ReadNumeric32();
        var strikePrice = reader.ReadPrice32();
        var expirationDate = reader.ReadDate();
        var decimalsInStrikePrice = reader.ReadNumeric16();
        var optionType = reader.ReadNumeric8();
        var exchangeCode = reader.ReadNumeric8();
        var marketCode = reader.ReadNumeric8();
        var priceQuotationFactor = reader.ReadPrice64();
        var corporateActionCode = reader.ReadAlpha(32);
        var notificationSign = reader.ReadAlpha(32);
        var otherSign = reader.ReadAlpha(32);
        var allowNvdr = reader.ReadAlpha(1);
        var allowShortSell = reader.ReadAlpha(1);
        var allowShortSellOnNvdr = reader.ReadAlpha(1);
        var allowTtf = reader.ReadAlpha(1);
        var parValue = reader.ReadNumeric64();
        var firstTradingDate = reader.ReadDate();
        var firstTradingTime = reader.ReadTime();
        var lastTradingDate = reader.ReadDate();
        var lastTradingTime = reader.ReadTime();
        var marketSegment = reader.ReadAlpha(4);
        var physicalDelivery = reader.ReadAlpha(1);
        var contractSize = reader.ReadNumeric32();
        var sectorCode = reader.ReadAlpha(4);
        var originatesFrom = reader.ReadAlpha(32);
        var status = reader.ReadAlpha(1);
        var modifier = reader.ReadNumeric16();
        var notationDate = reader.ReadDate();
        var decimalsInContractSizePqf = reader.ReadNumeric16();

        strikePrice.NumberOfDecimals = decimalsInStrikePrice;
        priceQuotationFactor.NumberOfDecimals = decimalsInContractSizePqf;

        var orderBookDirectoryParams = new OrderBookDirectoryParams
        {
            Nanos = nanos,
            OrderBookID = orderBookId,
            Symbol = symbol,
            LongName = longName,
            Isin = isin,
            FinancialProduct = financialProduct,
            TradingCurrency = tradingCurrency,
            DecimalsInPrice = decimalsInPrice,
            DecimalsInNominalValue = decimalsInNominalValue,
            RoundLotSize = roundLotSize,
            NominalValue = nominalValue,
            NumberOfLegs = numberOfLegs,
            UnderlyingName = underlyingName,
            Underlying = underlying,
            UnderlyingOrderBookID = underlyingOrderBookId,
            StrikePrice = strikePrice,
            ExpirationDate = expirationDate,
            DecimalsInStrikePrice = decimalsInStrikePrice,
            OptionType = optionType,
            ExchangeCode = exchangeCode,
            MarketCode = marketCode,
            PriceQuotationFactor = priceQuotationFactor,
            CorporateActionCode = corporateActionCode,
            NotificationSign = notificationSign,
            OtherSign = otherSign,
            AllowNvdr = allowNvdr,
            AllowShortSell = allowShortSell,
            AllowShortSellOnNvdr = allowShortSellOnNvdr,
            AllowTtf = allowTtf,
            ParValue = parValue,
            FirstTradingDate = firstTradingDate,
            FirstTradingTime = firstTradingTime,
            LastTradingDate = lastTradingDate,
            LastTradingTime = lastTradingTime,
            MarketSegment = marketSegment,
            PhysicalDelivery = physicalDelivery,
            ContractSize = contractSize,
            SectorCode = sectorCode,
            OriginatesFrom = originatesFrom,
            Status = status,
            Modifier = modifier,
            NotationDate = notationDate,
            DecimalsInContractSizePQF = decimalsInContractSizePqf
        };
        return new OrderBookDirectoryMessage(orderBookDirectoryParams);
    }

    public string ToStringUnitTest()
    {
        return $"OrderBookDirectoryMessage:\n"
               + $"Nanos: {Nanos},\n"
               + $"OrderBookID: {OrderBookID},\n"
               + $"Symbol: {Symbol},\n"
               + $"LongName: {LongName},\n"
               + $"ISIN: {ISIN},\n"
               + $"FinancialProduct: {FinancialProduct},\n"
               + $"TradingCurrency: {TradingCurrency},\n"
               + $"DecimalsInPrice: {DecimalsInPrice},\n"
               + $"DecimalsInNominalValue: {DecimalsInNominalValue},\n"
               + $"RoundLotSize: {RoundLotSize},\n"
               + $"NominalValue: {NominalValue},\n"
               + $"NumberOfLegs: {NumberOfLegs},\n"
               + $"UnderlyingName: {UnderlyingName},\n"
               + $"Underlying: {Underlying},\n"
               + $"UnderlyingOrderBookID: {UnderlyingOrderBookID},\n"
               + $"StrikePrice: {StrikePrice},\n"
               + $"ExpirationDate: {ExpirationDate},\n"
               + $"DecimalsInStrikePrice: {DecimalsInStrikePrice},\n"
               + $"OptionType: {OptionType},\n"
               + $"ExchangeCode: {ExchangeCode},\n"
               + $"MarketCode: {MarketCode},\n"
               + $"PriceQuotationFactor: {PriceQuotationFactor},\n"
               + $"CorporateActionCode: {CorporateActionCode},\n"
               + $"NotificationSign: {NotificationSign},\n"
               + $"OtherSign: {OtherSign},\n"
               + $"AllowNvdr: {AllowNvdr},\n"
               + $"AllowShortSell: {AllowShortSell},\n"
               + $"AllowShortSellOnNvdr: {AllowShortSellOnNvdr},\n"
               + $"AllowTtf: {AllowTtf},\n"
               + $"ParValue: {ParValue},\n"
               + $"FirstTradingDate: {FirstTradingDate},\n"
               + $"FirstTradingTime: {FirstTradingTime},\n"
               + $"LastTradingDate: {LastTradingDate},\n"
               + $"LastTradingTime: {LastTradingTime},\n"
               + $"MarketSegment: {MarketSegment},\n"
               + $"PhysicalDelivery: {PhysicalDelivery},\n"
               + $"ContractSize: {ContractSize},\n"
               + $"SectorCode: {SectorCode},\n"
               + $"OriginatesFrom: {OriginatesFrom},\n"
               + $"Status: {Status},\n"
               + $"Modifier: {Modifier},\n"
               + $"NotationDate: {NotationDate},\n"
               + $"DecimalsInContractSizePQF: {DecimalsInContractSizePQF}";
    }
}
using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Application.Services.Types.ItchParser;
using Pi.SetMarketData.Application.Utils;

namespace Pi.SetMarketData.Application.Services.Models.ItchParser;

public class OrderBookDirectoryParams
{
    public Numeric32 Nanos { get; set; }
    public Numeric32 OrderBookID { get; set; }
    public Alpha Symbol { get; set; }
    public Alpha LongName { get; set; }
    public Alpha Isin { get; set; }
    public Alpha FinancialProduct { get; set; }
    public Alpha TradingCurrency { get; set; }
    public Numeric16 DecimalsInPrice { get; set; }
    public Numeric16 DecimalsInNominalValue { get; set; }
    public Numeric32 RoundLotSize { get; set; }
    public Numeric64 NominalValue { get; set; }
    public Numeric8 NumberOfLegs { get; set; }
    public Alpha UnderlyingName { get; set; }
    public Numeric32 Underlying { get; set; }
    public Numeric32 UnderlyingOrderBookID { get; set; }
    public Price32 StrikePrice { get; set; }
    public Date ExpirationDate { get; set; }
    public Numeric16 DecimalsInStrikePrice { get; set; }
    public Numeric8 OptionType { get; set; }
    public Numeric8 ExchangeCode { get; set; }
    public Numeric8 MarketCode { get; set; }
    public Price64 PriceQuotationFactor { get; set; }
    public Alpha CorporateActionCode { get; set; }
    public Alpha NotificationSign { get; set; }
    public Alpha OtherSign { get; set; }
    public Alpha AllowNvdr { get; set; }
    public Alpha AllowShortSell { get; set; }
    public Alpha AllowShortSellOnNvdr { get; set; }
    public Alpha AllowTtf { get; set; }
    public Numeric64 ParValue { get; set; }
    public Date FirstTradingDate { get; set; }
    public Time FirstTradingTime { get; set; }
    public Date LastTradingDate { get; set; }
    public Time LastTradingTime { get; set; }
    public Alpha MarketSegment { get; set; }
    public Alpha PhysicalDelivery { get; set; }
    public Numeric32 ContractSize { get; set; }
    public Alpha SectorCode { get; set; }
    public Alpha OriginatesFrom { get; set; }
    public Alpha Status { get; set; }
    public Numeric16 Modifier { get; set; }
    public Date NotationDate { get; set; }
    public Numeric16 DecimalsInContractSizePQF { get; set; }
}

public class OrderBookDirectoryMessage : ItchMessage
{
    public Numeric32 Nanos { get; private set; }
    public Numeric32 OrderBookID { get; private set; }
    public Alpha Symbol { get; private set; }
    public Alpha LongName { get; private set; }
    public Alpha ISIN { get; private set; }
    public Alpha FinancialProduct { get; private set; }
    public Alpha TradingCurrency { get; private set; }
    public Numeric16 DecimalsInPrice { get; private set; }
    public Numeric16 DecimalsInNominalValue { get; private set; }
    public Numeric32 RoundLotSize { get; private set; }
    public Numeric64 NominalValue { get; private set; }
    public Numeric8 NumberOfLegs { get; private set; }
    public Alpha UnderlyingName { get; private set; }
    public Numeric32 Underlying { get; private set; }
    public Numeric32 UnderlyingOrderBookID { get; private set; }
    public Price32 StrikePrice { get; private set; }
    public Date ExpirationDate { get; private set; }
    public Numeric16 DecimalsInStrikePrice { get; private set; }
    public Numeric8 OptionType { get; private set; }
    public Numeric8 ExchangeCode { get; private set; }
    public Numeric8 MarketCode { get; private set; }
    public Price64 PriceQuotationFactor { get; private set; }
    public Alpha CorporateActionCode { get; private set; }
    public Alpha NotificationSign { get; private set; }
    public Alpha OtherSign { get; private set; }
    public Alpha AllowNvdr { get; private set; }
    public Alpha AllowShortSell { get; private set; }
    public Alpha AllowShortSellOnNvdr { get; private set; }
    public Alpha AllowTtf { get; private set; }
    public Numeric64 ParValue { get; private set; }
    public Date FirstTradingDate { get; private set; }
    public Time FirstTradingTime { get; private set; }
    public Date LastTradingDate { get; private set; }
    public Time LastTradingTime { get; private set; }
    public Alpha MarketSegment { get; private set; }
    public Alpha PhysicalDelivery { get; private set; }
    public Numeric32 ContractSize { get; private set; }
    public Alpha SectorCode { get; private set; }
    public Alpha OriginatesFrom { get; private set; }
    public Alpha Status { get; private set; }
    public Numeric16 Modifier { get; private set; }
    public Date NotationDate { get; private set; }
    public Numeric16 DecimalsInContractSizePQF { get; private set; }

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

    public static OrderBookDirectoryMessage Parse(byte[] bytes)
    {
        if (bytes == null || bytes.Length < 320) // Ensure the byte array length is as expected
        {
            throw new ArgumentException("Invalid data format for OrderBookDirectoryMessage.");
        }

        using (var reader = new ItchMessageByteReader(new Memory<byte>(bytes)))
        {
            var nanos = reader.ReadNumeric32();
            var orderBookID = reader.ReadNumeric32();
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
            var underlyingOrderBookID = reader.ReadNumeric32();
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
            var decimalsInContractSizePQF = reader.ReadNumeric16();

            strikePrice.NumberOfDecimals = decimalsInStrikePrice;
            priceQuotationFactor.NumberOfDecimals = decimalsInContractSizePQF;

            var orderBookDirectoryParams = new OrderBookDirectoryParams
            {
                Nanos = nanos,
                OrderBookID = orderBookID,
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
                UnderlyingOrderBookID = underlyingOrderBookID,
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
                DecimalsInContractSizePQF = decimalsInContractSizePQF
            };
            return new OrderBookDirectoryMessage(orderBookDirectoryParams);
        }
    }

    public override string ToString()
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

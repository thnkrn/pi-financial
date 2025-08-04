using Pi.SetMarketDataWSS.Application.Services.Constants;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;

namespace Pi.SetMarketDataWSS.Application.Services.Models.ItchMessageWrapper;

public class OrderBookDirectoryMessageWrapper : ItchMessage
{
    public OrderBookDirectoryMessageWrapper()
    {
        MsgType = ItchMessageType.R;
    }

    public Nanos? Nanos { get; set; }
    public OrderBookId? OrderBookID { get; set; }
    public Symbol? Symbol { get; set; }
    public LongName? LongName { get; set; }
    public Isin? ISIN { get; set; }
    public FinancialProduct? FinancialProduct { get; set; }
    public TradingCurrency? TradingCurrency { get; set; }
    public DecimalsInPrice? DecimalsInPrice { get; set; }
    public DecimalsInNominalValue? DecimalsInNominalValue { get; set; }
    public RoundLotSize? RoundLotSize { get; set; }
    public NominalValue? NominalValue { get; set; }
    public NumberOfLegs? NumberOfLegs { get; set; }
    public UnderlyingName? UnderlyingName { get; set; }
    public Underlying? Underlying { get; set; }
    public UnderlyingOrderBookId? UnderlyingOrderBookID { get; set; }
    public StrikePrice? StrikePrice { get; set; }
    public ExpirationDate? ExpirationDate { get; set; }
    public DecimalsInStrikePrice? DecimalsInStrikePrice { get; set; }
    public OptionType? OptionType { get; set; }
    public ExchangeCode? ExchangeCode { get; set; }
    public MarketCode? MarketCode { get; set; }
    public PriceQuotationFactor? PriceQuotationFactor { get; set; }
    public CorporateActionCode? CorporateActionCode { get; set; }
    public NotificationSign? NotificationSign { get; set; }
    public OtherSign? OtherSign { get; set; }
    public AllowNvdr? AllowNvdr { get; set; }
    public AllowShortSell? AllowShortSell { get; set; }
    public AllowShortSellOnNvdr? AllowShortSellOnNvdr { get; set; }
    public AllowTtf? AllowTtf { get; set; }
    public ParValue? ParValue { get; set; }
    public FirstTradingDate? FirstTradingDate { get; set; }
    public FirstTradingTime? FirstTradingTime { get; set; }
    public LastTradingDate? LastTradingDate { get; set; }
    public LastTradingTime? LastTradingTime { get; set; }
    public MarketSegment? MarketSegment { get; set; }
    public PhysicalDelivery? PhysicalDelivery { get; set; }
    public ContractSize? ContractSize { get; set; }
    public SectorCode? SectorCode { get; set; }
    public OriginatesFrom? OriginatesFrom { get; set; }
    public Status? Status { get; set; }
    public Modifier? Modifier { get; set; }
    public NotationDate? NotationDate { get; set; }
    public DecimalsInContractSizePqf? DecimalsInContractSizePQF { get; set; }
    public Metadata? Metadata { get; set; }

    public new char MsgType
    {
        get => base.MsgType;
        private set => base.MsgType = value;
    }
}
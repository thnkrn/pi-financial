using System.Globalization;
using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Application.Helper;
using Pi.SetMarketData.Application.Interfaces.ItchMapper;
using Pi.SetMarketData.Application.Models.ItchMessageWrapper;
using Pi.SetMarketData.Application.Services.Models.ItchParser;
using Pi.SetMarketData.Domain.Entities;

namespace Pi.SetMarketData.Application.Services.ItchMapper
{
    public class ItchOrderBookDirectoryService : IItchOrderBookDirectoryMapperService
    {
        private readonly HashSet<string> _equityType = ["CS", "CSF", "PS", "PSF", "W", "TSR", "DWC", "DWP", "DR", "ETF", "UT", "UL"];
        private readonly HashSet<string> _derivativeType = ["FC", "FP", "OEC", "OEP", "WEC", "WEP", "CMB", "SPT"];
        public Instrument? MapToInstrument(ItchMessage? message, MarketDirectory? marketDirectory)
        {
            if (message is not OrderBookDirectoryMessageWrapper wrapper)
                return null;

            var instrument = new Instrument();

            var financialProduct = wrapper.FinancialProduct.Value.ToUpper();
            var marketSegment = InstrumentCategoryHelper.MapMarketSegment(
                wrapper.MarketSegment.Value
            );
            var marketCode = InstrumentCategoryHelper.MapMarketCode(wrapper.MarketCode.Value);
            var underlyingName = wrapper.UnderlyingName.Value.ToUpper();
            var instrumentCategory = InstrumentCategoryHelper.MapInstrumentCategory(
                financialProduct,
                marketSegment,
                marketCode,
                underlyingName
            );

            if (instrumentCategory == InstrumentConstants.Ignored)
            {
                return null;
            }

            if (marketDirectory != null)
                instrument.TradingMarket = marketDirectory.MarketDescription;

            instrument.Symbol = wrapper.Symbol.Value;
            instrument.OrderBookId = wrapper.OrderBookID.Value;
            if (wrapper.UnderlyingOrderBookID.Value.HasValue)
                instrument.UnderlyingOrderBookID = wrapper.UnderlyingOrderBookID.Value;
            instrument.InstrumentCategory = instrumentCategory;
            instrument.LongName = wrapper.LongName.Value;
            instrument.Market = marketSegment;
            instrument.MinimumOrderSize = wrapper.RoundLotSize.Value.ToString();
            instrument.Multiplier = wrapper.PriceQuotationFactor.Value.ToString();
            instrument.SecurityType = wrapper.FinancialProduct.Value.ToString();
            instrument.Currency = (instrument.SecurityType == InstrumentConstants.IDX ||
                (marketSegment == InstrumentConstants.TXI && InstrumentConstants.CurrencyTHB == wrapper.TradingCurrency.Value))
                    ? "points" : wrapper.TradingCurrency.Value;
            if (wrapper.LastTradingDate.Value.HasValue
            && (wrapper.LastTradingDate.Value != DateTime.MinValue)
            && !(instrument.FromSetSmart.HasValue && instrument.FromSetSmart.Value)
            )
            {
                instrument.LastTradingDate = wrapper.LastTradingDate.Value.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            instrument.InstrumentType = instrument.SecurityType switch
            {
                "IDX" => "Index",
                var securityType when _equityType.Contains(securityType) => "Equity",
                var securityType when _derivativeType.Contains(securityType) => "Derivative",
                _ => ""
            };
            instrument.Venue = VenueHelper.GetVenue(instrument.SecurityType);

            if (instrument.SecurityType == "IDX")
            {
                var friendlyName = instrument.Symbol switch
                {
                    "SET" => "SET Index",
                    "SET50" => "SET50 Index",
                    "MAI" => "MAI Index",
                    _ => ""
                };
                if (!string.IsNullOrEmpty(friendlyName)) instrument.FriendlyName = friendlyName;
            }

            instrument.Deprecated = IsDeprecated(instrument, wrapper);

            return instrument;
        }

        public InstrumentDetail? MapToInstrumentDetail(ItchMessage? message)
        {
            if (message == null)
                return null;

            var instrumentDetail = new InstrumentDetail();
            var wrapper = (OrderBookDirectoryMessageWrapper)message;
            instrumentDetail.DecimalsInPrice = wrapper.DecimalsInPrice.Value;
            instrumentDetail.DecimalInStrikePrice = wrapper.DecimalsInStrikePrice.Value;
            instrumentDetail.DecimalInContractSizePQF = wrapper.DecimalsInContractSizePQF.Value;
            instrumentDetail.StrikePrice = wrapper.StrikePrice.Value.ToString();
            instrumentDetail.ContractSize = wrapper.ContractSize.Value.ToString();
            instrumentDetail.InstrumentId = wrapper.OrderBookID.Value;
            instrumentDetail.OrderBookId = wrapper.OrderBookID.Value;
            instrumentDetail.UnderlyingOrderBookID = wrapper.UnderlyingOrderBookID.Value;

            return instrumentDetail;
        }

        public OrderBook? MapToOrderBook(ItchMessage? message)
        {
            if (message == null)
                return null;

            var orderBook = new OrderBook();
            var wrapper = (OrderBookDirectoryMessageWrapper)message;
            orderBook.OrderBookId = wrapper.OrderBookID.Value;
            orderBook.Symbol = wrapper.Symbol.Value;

            return orderBook;
        }

        public WhiteList? MapToWhiteList(ItchMessage? message, bool? exchangeServer)
        {
            if (message == null)
                return null;
            var whiteList = new WhiteList();
            var wrapper = (OrderBookDirectoryMessageWrapper)message;

            var financialProduct = wrapper.FinancialProduct.Value.ToUpper();
            var marketSegment = InstrumentCategoryHelper.MapMarketSegment(
                wrapper.MarketSegment.Value
            );
            var marketCode = InstrumentCategoryHelper.MapMarketCode(wrapper.MarketCode.Value);
            var underlyingName = wrapper.UnderlyingName.Value.ToUpper();

            var symbol = InstrumentCategoryHelper.MapInstrumentCategory(
                financialProduct,
                marketSegment,
                marketCode,
                underlyingName
            );

            if (symbol is InstrumentConstants.Unstructured or InstrumentConstants.Ignored)
            {
                return null;
            }

            whiteList.Symbol = symbol;
            whiteList.IsWhitelist = true; // Default value is true
            whiteList.Mic = InstrumentConstants.BKK;
            whiteList.StandardTicker = symbol;
            whiteList.Exchange = exchangeServer is null or true
                ? InstrumentConstants.SET
                : InstrumentConstants.TFEX;

            return whiteList;
        }

        public CorporateAction? MapToCorporateAction(ItchMessage? message)
        {
            if (message is not OrderBookDirectoryMessageWrapper wrapper)
                return null;

            var corporateAction = new CorporateAction
            {
                Code = string.Empty,
                Date = DateTime.Now.ToString(DataFormat.YearMonthDayFormat),
                OrderBookId = wrapper.OrderBookID.Value
            };
            if (!string.IsNullOrEmpty(wrapper.CorporateActionCode.Value))
                corporateAction.Code = wrapper.CorporateActionCode.Value;

            return corporateAction;
        }

        public TradingSign? MapToTradingSign(ItchMessage? message)
        {
            if (message is not OrderBookDirectoryMessageWrapper wrapper)
                return null;

            var notificationSign = (wrapper.NotificationSign.Value ?? string.Empty).Trim();
            var otherSign = (wrapper.OtherSign.Value ?? string.Empty).Trim();

            var sign = (
                string.IsNullOrEmpty(notificationSign),
                string.IsNullOrEmpty(otherSign)
            ) switch
            {
                (true, true) => string.Empty,
                (false, true) => notificationSign,
                (true, false) => otherSign,
                (false, false) => $"{notificationSign},{otherSign}"
            };

            return new TradingSign { Sign = sign, OrderBookId = wrapper.OrderBookID.Value, };
        }

        public static bool IsDeprecated(Instrument instrument, OrderBookDirectoryMessageWrapper wrapper)
        {
            // Check Equity
            if (instrument.SecurityType is InstrumentConstants.CS or InstrumentConstants.PS or InstrumentConstants.ETF
                or InstrumentConstants.DR)
            {
                var notificationSign = (wrapper.NotificationSign.Value ?? string.Empty).Trim();
                var otherSign = (wrapper.OtherSign.Value ?? string.Empty).Trim();
                
                return notificationSign.Equals("X", StringComparison.OrdinalIgnoreCase) || otherSign.Equals("X", StringComparison.OrdinalIgnoreCase);
            }

            return InstrumentHelper.IsDeprecated(instrument);
        }
    }
}

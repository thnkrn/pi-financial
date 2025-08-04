using System.Globalization;
using Pi.GlobalMarketData.Application.Utils;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Domain.Models.Response;

namespace Pi.GlobalMarketData.Application.Services.MarketData.MarketTicker;

public class MarketTickerService
{
    private List<ExchangeTimezone> _exchangeTimezoneList = [];
    private List<double> _high52W = [];
    private List<string> _instrumentCategory = [];
    private List<GeInstrument> _instrumentList = [];
    private List<string> _logos = [];
    private List<double> _low52W = [];
    private List<MorningStarStocks> _morningStarStocksList = [];
    private List<StreamingBody> _streamingBodyList = [];
    private List<string> _symbol = [];
    private List<string> _venue = [];
    private List<MarketSchedule> _marketSchedule = [];

    public MarketTickerService SetTickerPriceParams(List<double> high52W, List<double> low52W)
    {
        _high52W = high52W;
        _low52W = low52W;

        return this;
    }

    public MarketTickerService SetTickerParams(
        List<string> symbol,
        List<string> venue,
        List<GeInstrument> instrumentList,
        List<ExchangeTimezone> exchangeTimezoneList,
        List<MorningStarStocks> morningStarStocksList,
        List<StreamingBody> streamingBodyList
    )
    {
        _symbol = symbol;
        _venue = venue;
        _instrumentList = instrumentList;
        _exchangeTimezoneList = exchangeTimezoneList;
        _morningStarStocksList = morningStarStocksList;
        _streamingBodyList = streamingBodyList;

        return this;
    }

    public MarketTickerService SetLogoParams(List<string> logos)
    {
        _logos = logos;
        return this;
    }

    public MarketTickerService SetInstrumentCategory(List<string> instrumentCategory)
    {
        _instrumentCategory = instrumentCategory;

        return this;
    }

    public MarketTickerService SetMarketSchedule(List<MarketSchedule> marketSchedule)
    {
        _marketSchedule = marketSchedule;
        return this;
    }

    public MarketTickerResponse GetResult()
    {
        List<TickerBody> data = [];

        for (var i = 0; i < _symbol.Count; i++)
        {
            var instrument = _instrumentList[i];
            var exchangeTimezone = _exchangeTimezoneList[i];
            var morningStarStocks = _morningStarStocksList[i];
            var streamingBody = _streamingBodyList[i];
            var marketSchedule = _marketSchedule[i];
            var tickerBody = new TickerBody
            {
                Symbol = _symbol[i],
                Venue = _venue[i],
                Price = string.IsNullOrEmpty(streamingBody.Price) ?
                    "0.00" :
                    streamingBody.Price,
                Currency = instrument.Currency ?? string.Empty,
                AuctionPrice = streamingBody.AuctionPrice ?? "0.00",
                AuctionVolume = streamingBody.AuctionVolume ?? "0.00",
                Open = streamingBody.Open ?? "0",
                High24H = streamingBody.High24H ?? "0.00",
                Low24H = streamingBody.Low24H ?? "0.00",
                High52W = _high52W[i].ToString(CultureInfo.InvariantCulture),
                Low52W = _low52W[i].ToString(CultureInfo.InvariantCulture),
                PriceChanged = string.IsNullOrEmpty(streamingBody.PriceChanged) ?
                    "0.00" :
                    streamingBody.PriceChanged,
                PriceChangedRate = string.IsNullOrEmpty(streamingBody.PriceChangedRate) ? 
                    "0.00" : 
                    streamingBody.PriceChangedRate,
                Volume = string.IsNullOrEmpty(streamingBody.Volume) ?
                    "0" :
                    streamingBody.Volume,
                Amount =  string.IsNullOrEmpty(streamingBody.Amount) ? 
                    "0.00" :
                    streamingBody.Amount,
                ChangeAmount = string.Empty,
                ChangeVolume = "0",
                TurnoverRate = string.Empty,
                Open2 = streamingBody.Open2 ?? string.Empty,
                Ceiling = streamingBody.Ceiling ?? "0",
                Floor = streamingBody.Floor ?? "0",
                Average = streamingBody.Average ?? "0",
                AverageBuy = streamingBody.AverageBuy ?? "0",
                AverageSell = streamingBody.AverageSell ?? "0",
                Aggressor = streamingBody.Aggressor ?? string.Empty,
                PreClose = streamingBody.PreClose ?? "0",
                Status = marketSchedule.MarketSession ?? "",
                Yield = morningStarStocks.DividendYield.ToString(CultureInfo.InvariantCulture),
                Pe = morningStarStocks.PriceToEarningsRatio.ToString(CultureInfo.InvariantCulture),
                Pb = morningStarStocks.PriceToBookRatio.ToString(CultureInfo.InvariantCulture),
                TotalAmount = streamingBody.TotalAmount ?? "0.00",
                TotalAmountK = streamingBody.TotalAmountK ?? "0",
                TotalVolume = streamingBody.TotalVolume ?? "0",
                TotalVolumeK = streamingBody.TotalVolumeK ?? "0",
                TradableEquity = "0",
                TradableAmount = string.Empty,
                Eps =
                    morningStarStocks.EarningsPerShares.Values.Count > 0
                        ? morningStarStocks.EarningsPerShares.Values[0].ToString(CultureInfo.InvariantCulture)
                        : "0",
                PublicTrades = streamingBody.PublicTrades ?? [],
                OrderBook = new TickerOrderBook
                {
                    Bid = streamingBody.OrderBook?.Bid ?? [],
                    Offer = streamingBody.OrderBook?.Offer ?? []
                },
                SecurityType = "GlobalEquity",
                InstrumentType = "GlobalEquity",
                Market = string.Empty,
                LastTrade = streamingBody.LastTrade ?? string.Empty,
                ToLastTrade = streamingBody.ToLastTrade,
                Moneyness = streamingBody.Moneyness ?? string.Empty,
                MaturityDate = string.Empty,
                Multiplier = "0",
                ExercisePrice = string.Empty,
                IntrinsicValue = "0",
                PSettle = "0",
                Poi = streamingBody.Poi ?? "0",
                Underlying = "0",
                Open0 = streamingBody.Open0 ?? string.Empty,
                Open1 = streamingBody.Open1 ?? string.Empty,
                Basis = streamingBody.Basis ?? "0",
                Settle = "0",
                InstrumentCategory = DataManipulation.RemoveSpace(
                    _instrumentCategory[i]
                ),
                FriendlyName = instrument.Name,
                Logo = _logos.ElementAtOrDefault(i) ?? string.Empty,
                ExchangeTimezone = exchangeTimezone.Timezone,
                Country = instrument.Country ?? string.Empty,
                OffsetSeconds = DataManipulation.GetOffset(
                    exchangeTimezone.Timezone
                ),
                IsProjected = !new HashSet<string> { "MainSession", "AfterMarket" }.Contains(marketSchedule.MarketSession ?? ""),
                LastPriceTime = streamingBody.LastPriceTime
            };

            data.Add(tickerBody);
        }

        return new MarketTickerResponse
        {
            Code = "0",
            Message = string.Empty,
            Response = new TickerResponse { Data = data }
        };
    }
}
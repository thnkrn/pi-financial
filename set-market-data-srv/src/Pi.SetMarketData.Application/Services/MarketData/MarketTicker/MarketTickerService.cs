using System.Globalization;
using Pi.SetMarketData.Application.Utils;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Domain.Models.Response;

namespace Pi.SetMarketData.Application.Services.MarketData.MarketTicker;

public class MarketTickerServiceParams
{
    public List<double> High52WList { get; set; } = [];
    public List<double> Low52WList { get; set; } = [];
}

public class MarketTickerService
{
    private List<PriceResponse> _underlyingPriceResponse = [];
    private List<string> _venue;

    public MarketTickerService SetUnderlyingPriceResponse(List<PriceResponse> underlyingPriceResponse)
    {
        _underlyingPriceResponse = underlyingPriceResponse;
        return this;
    }

    public MarketTickerService SetVenue(List<string> venue)
    {
        _venue = venue;
        return this;
    }

    public MarketTickerResponse GetResult(
        List<MarketStreamingResponse> marketStreamings,
        MarketTickerServiceParams marketTickerParams,
        List<Instrument> instruments,
        List<InstrumentDetail> instrumentDetails,
        List<MorningStarStocks> morningStarStocksList,
        List<ExchangeTimezone> exchangeTimezones,
        List<string> logos
    )
    {
        List<TickerBody> data = [];

        for (var i = 0; i < instrumentDetails.Count; i++)
        {
            var marketStreaming = marketStreamings[i].Response?.Data?.FirstOrDefault() ?? new StreamingBody();
            var underlyingPriceResponse = _underlyingPriceResponse[i] ?? new PriceResponse();
            var instrument = instruments[i];
            var instrumentDetail = instrumentDetails[i];
            var morningStarStocks = morningStarStocksList[i];
            var exchangeTimezone = exchangeTimezones[i];
            var high52W = marketTickerParams.High52WList[i].ToString(CultureInfo.InvariantCulture) ?? "0.00";
            var low52W = marketTickerParams.Low52WList[i].ToString(CultureInfo.InvariantCulture) ?? "0.00";
            var venue = _venue[i];

            var tickerBody = new TickerBody
            {
                Symbol = instrument.Symbol ?? string.Empty,
                Venue = venue,
                Price = string.IsNullOrEmpty(marketStreaming.Price) ? "0.00" : marketStreaming.Price,
                Currency = instrument.Currency ?? string.Empty,
                AuctionPrice = marketStreaming.AuctionPrice ?? "0.00",
                AuctionVolume = "0", //TBC value from MsgZ, need to be handle first.
                Open = marketStreaming.Open ?? string.Empty,
                High24H = marketStreaming.High24H ?? "0.00",
                Low24H = marketStreaming.Low24H ?? "0.00",
                High52W = high52W,
                Low52W = low52W,
                PriceChanged = string.IsNullOrEmpty(marketStreaming.PriceChanged) ? "0.00" : marketStreaming.PriceChanged,
                PriceChangedRate = string.IsNullOrEmpty(marketStreaming.PriceChangedRate) ? "0.00" : marketStreaming.PriceChangedRate,
                Volume = string.IsNullOrEmpty(marketStreaming.Volume) ? "0" : marketStreaming.Volume,
                Amount = string.IsNullOrEmpty(marketStreaming.Amount) ? "0.00" : marketStreaming.Amount,
                ChangeAmount = "0.00", // TBD
                ChangeVolume = "0", // TBD
                TurnoverRate = "0.00", // TBD
                Open2 = marketStreaming.Open2 ?? string.Empty,
                Ceiling = marketStreaming.Ceiling ?? string.Empty,
                Floor = marketStreaming.Floor ?? string.Empty,
                Average = marketStreaming.Average ?? string.Empty,
                AverageBuy = marketStreaming.AverageBuy ?? "0.00",
                AverageSell = marketStreaming.AverageSell ?? "0.00",
                Aggressor = marketStreaming.Aggressor ?? string.Empty,
                PreClose = marketStreaming.PreClose ?? "0.00",
                Status = marketStreaming.Status ?? string.Empty,
                Yield = morningStarStocks.DividendYield.ToString(CultureInfo.InvariantCulture) ?? "0",
                Pe = morningStarStocks.PriceToEarningsRatio.ToString(CultureInfo.InvariantCulture) ?? "0.00",
                Pb = morningStarStocks.PriceToBookRatio.ToString(CultureInfo.InvariantCulture) ?? "0.00",
                TotalAmount = marketStreaming.TotalAmount ?? "0.00",
                TotalAmountK = marketStreaming.TotalAmountK ?? "0.00",
                TotalVolume = marketStreaming.TotalVolume ?? "0",
                TotalVolumeK = (marketStreaming.TotalVolume ?? "0").FormatDecimals(3),
                TradableEquity = "0", // TBD
                TradableAmount = "0.00", // TBD
                Eps = (
                    morningStarStocks.EarningsPerShares.Values.Count > 0
                        ? morningStarStocks.EarningsPerShares.Values[0].ToString(CultureInfo.InvariantCulture)
                        : "0"
                ).FormatDecimals(instrumentDetail.DecimalsInPrice),
                PublicTrades = marketStreaming?.PublicTrades ?? [],
                OrderBook = new TickerOrderBook
                {
                    Bid =
                        marketStreaming
                            ?.OrderBook?.Bid?.GroupBy(sublist => sublist[0])
                            .Select(group => group.First())
                            .Take(10)
                            .ToList() ?? [],
                    Offer =
                        marketStreaming
                            ?.OrderBook?.Offer?.GroupBy(sublist => sublist[0])
                            .Select(group => group.First())
                            .Take(10)
                            .ToList() ?? []
                },
                SecurityType = instrument.SecurityType ?? string.Empty,
                InstrumentType = instrument.InstrumentType ?? string.Empty,
                Market = instrument.Market,
                LastTrade = marketStreaming?.LastTrade ?? string.Empty,
                ToLastTrade = marketStreaming?.ToLastTrade ?? 0,
                Moneyness = marketStreaming?.Moneyness ?? string.Empty,
                MaturityDate = string.Empty, // TBD from SET SMART
                Multiplier = instrument.Multiplier ?? "0",
                ExercisePrice = marketStreaming?.ExercisePrice ?? "0",
                IntrinsicValue = "0", // TBD from SET SMART
                PSettle = venue.Equals("Equity", StringComparison.OrdinalIgnoreCase) ? "0" : marketStreaming?.PreClose ?? string.Empty,
                Poi = marketStreaming?.Poi ?? string.Empty,
                Underlying = underlyingPriceResponse.Price ?? "0",
                Open0 = marketStreaming?.Open0 ?? string.Empty,
                Open1 = marketStreaming?.Open1 ?? string.Empty,
                Basis = venue.Equals("Equity", StringComparison.OrdinalIgnoreCase) ||
                    new HashSet<string> { "TXA", "TXC", "TXM", "TXR" }.Contains(marketStreaming?.Market ?? "") ? "0" :
                    CalculateBasis(marketStreaming?.Price ?? "0", underlyingPriceResponse.Price ?? "0"),
                Settle = string.Empty, // TBD
                InstrumentCategory = DataManipulation.RemoveSpace(
                    instrument.InstrumentCategory ?? string.Empty
                ),
                FriendlyName = string.IsNullOrEmpty(instrument.FriendlyName)
                    ? instrument.LongName
                    : instrument.FriendlyName,
                Logo = logos.ElementAtOrDefault(i) ?? string.Empty,
                ExchangeTimezone = exchangeTimezone.Timezone ?? string.Empty,
                Country = string.IsNullOrEmpty(instrument.Symbol) ? string.Empty : "TH",
                OffsetSeconds = DataManipulation.GetOffset(
                    exchangeTimezone.Timezone ?? string.Empty
                ),
                IsProjected = marketStreaming?.IsProjected ?? false,
                LastPriceTime = marketStreaming?.LastPriceTime ?? 0
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


    private static string CalculateBasis(string price, string underlyingPrice)
    {
        if (double.TryParse(price, out double number1) && double.TryParse(underlyingPrice, out double number2))
        {
            double result = number1 - number2;
            return result.ToString("F2");
        }
        else
        {
            return price;
        }
    }
}

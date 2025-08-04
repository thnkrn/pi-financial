using System.Globalization;
using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Application.Utils;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Domain.Models.Response;

namespace Pi.SetMarketData.Application.Services.MarketData.MarketProfileFundamentals;

public class WarrantData
{
    public string ExerciseDate { get; set; } = string.Empty;
    public string DaysToExercise { get; set; } = string.Empty;
    public string MaturityDate { get; set; } = string.Empty;
    public string Moneyness { get; set; } = string.Empty;
    public string LastTradingDate { get; set; } = string.Empty;
    public string DaysToLastTrade { get; set; } = string.Empty;
}

public class MarketProfileFundamentalsService
{
    private Instrument? _instrument;
    private InstrumentDetail? _instrumentDetail;
    private string? _venue;
    private MarketStreamingResponse? _marketStreamingResponse;
    private MorningStarStocks? _morningStarStocks;
    private Instrument? _underlyingInstrument;
    private MarketStreamingResponse? _underlyingStreamingResponse;

    public MarketProfileFundamentalsService SetVenue(string venue)
    {
        _venue = venue;
        return this;
    }

    public MarketProfileFundamentalsService SetMorningStar(MorningStarStocks? morningStarStocks)
    {
        _morningStarStocks = morningStarStocks ?? new MorningStarStocks();
        return this;
    }

    public MarketProfileFundamentalsService SetInstrument(
        Instrument? instrument,
        Instrument? underlyingInstrument
    )
    {
        _instrument = instrument ?? new Instrument();
        _underlyingInstrument = underlyingInstrument ?? new Instrument();
        return this;
    }

    public MarketProfileFundamentalsService SetInstrumentDetail(InstrumentDetail? instrumentDetail)
    {
        _instrumentDetail = instrumentDetail ?? new InstrumentDetail();
        return this;
    }

    public MarketProfileFundamentalsService SetMarketStreaming(
        MarketStreamingResponse? marketStreamingResponse
    )
    {
        _marketStreamingResponse = marketStreamingResponse ?? new MarketStreamingResponse();
        return this;
    }

    public MarketProfileFundamentalsService SetUnderlying(
        MarketStreamingResponse? underlyingStreamingResponse
    )
    {
        _underlyingStreamingResponse = underlyingStreamingResponse;
        return this;
    }

    public MarketProfileFundamentalsResponse GetResult()
    {
        var morningStarStocks = _morningStarStocks ?? new MorningStarStocks();
        var streamingBody =
            _marketStreamingResponse?.Response?.Data?.FirstOrDefault() ?? new StreamingBody();
        var underlyingStreamingBody =
            _underlyingStreamingResponse?.Response?.Data?.FirstOrDefault() ?? new StreamingBody();

        var industry = morningStarStocks.Industry;

        var sector = morningStarStocks.Sector;

        var country = DataManipulation.GetCountryName(morningStarStocks.Country);

        var priceToEarningsRatio =
            morningStarStocks.PriceToEarningsRatio.ToString(CultureInfo.InvariantCulture)
            ?? string.Empty;

        var priceToBookRatio =
            morningStarStocks.PriceToBookRatio.ToString(CultureInfo.InvariantCulture)
            ?? string.Empty;

        var priceToSalesRatio =
            morningStarStocks.PriceToSalesRatio.ToString(CultureInfo.InvariantCulture)
            ?? string.Empty;

        var convertedMarketCapitalization =
            Math.Round(
                    morningStarStocks.MarketCapitalization / 1000000000.0,
                    3,
                    MidpointRounding.AwayFromZero
                )
                .ToString("F3", CultureInfo.InvariantCulture) ?? "0.000";

        var exDividendDate = DataManipulation.DateToString(
            morningStarStocks.ExDividendDate,
            DataFormat.DayMonthYearFormat
        );

        var units = _instrument?.Currency;

        var source =
            _instrument?.InstrumentCategory == InstrumentConstants.ThaiStocks
                ? "MorningStar"
                : "SETtrade";

        var dividendYieldPercentage =
            Math.Round(morningStarStocks.DividendYield * 100, 2)
                .ToString(CultureInfo.InvariantCulture) ?? string.Empty;

        var exercisePrice = _instrument?.ExercisePrice;
        var underlyingPrice = underlyingStreamingBody?.Price;
        var strikePrice = streamingBody?.ExercisePrice ?? "0";
        var moneyness = "";

        var exerciseDate = "";
        var daysToExercise = "";
        var maturityDate = "";
        var lastTradingDate = "";
        var daysToLastTrade = "";
        if (_instrument?.InstrumentCategory == InstrumentConstants.ThaiStockWarrants) // Warrants
        {
            var warrantData = MapWarrants(streamingBody);
            exerciseDate = warrantData.ExerciseDate;
            daysToExercise = warrantData.DaysToExercise;
            maturityDate = warrantData.ExerciseDate;
            moneyness = warrantData.Moneyness;
        }
        else if (_instrument?.InstrumentCategory == InstrumentConstants.ThaiDerivativeWarrants) // DWs
        {
            var warrantData = MapDWs(underlyingStreamingBody);
            maturityDate = warrantData.MaturityDate;
            daysToLastTrade = warrantData.DaysToLastTrade;
            lastTradingDate = warrantData.LastTradingDate;
            moneyness = warrantData.Moneyness;
        }
        else
        {
            var tradingDate = "";
            if (DataManipulation.TryParseDateTime(_instrument?.LastTradingDate, out var parsedDate))
            {
                if (parsedDate != DateTime.MinValue)
                {
                    tradingDate = _instrument?.LastTradingDate ?? string.Empty;
                }
            }
            lastTradingDate = DataManipulation.ToDayMonthYear(tradingDate);
            daysToLastTrade = DataManipulation.CalculateDaysUntilDate(tradingDate);
        }

        return new MarketProfileFundamentalsResponse
        {
            Code = "0",
            Message = string.Empty,
            Response = new ProfileFundamentalsResponse
            {
                MarketCapitalization = convertedMarketCapitalization ?? "0.000",
                ShareFreeFloat = _instrumentDetail?.ShareFreeFloat ?? "0",
                Industry = industry,
                Sector = sector,
                Country = country,
                PriceToEarningsRatio = priceToEarningsRatio,
                PriceToBookRatio = priceToBookRatio,
                PriceToSalesRatio = priceToSalesRatio,
                DividendYieldPercentage = dividendYieldPercentage,
                ExDividendDate = exDividendDate,
                Units = units,
                Source = source ?? "",
                UnderlyingSymbol = _underlyingInstrument?.Symbol ?? "",
                IsClickable = _underlyingInstrument?.Symbol != null,
                UnderlyingVenue = _underlyingInstrument?.Venue ?? "",
                UnderlyingInstrumentType = _underlyingInstrument?.InstrumentType ?? "",
                UnderlyingInstrumentCategory = DataManipulation.RemoveSpace(
                    _underlyingInstrument?.InstrumentCategory ?? string.Empty
                ),
                UnderlyingLogo = _underlyingInstrument?.Logo ?? "",
                ExerciseRatio = _instrument?.ExerciseRatio ?? "0 : 0",
                ExercisePrice = DataManipulation.RemoveTrailingZeros(exercisePrice ?? "0"),
                ExerciseDate = exerciseDate,
                DaysToExercise = daysToExercise,
                Moneyness = moneyness,
                Direction = _instrument?.Direction ?? "",
                Multiplier = _instrument?.Multiplier ?? "0",
                LastTradingDate = lastTradingDate,
                DaysToLastTrade = daysToLastTrade,
                MaturityDate = maturityDate,
                IssuerSeries = _instrument?.IssuerSeries ?? "",
                ForeignCurrency = _underlyingInstrument?.Currency ?? "",
                ConversionRatio = _instrument?.ConversionRatio ?? "0 : 0",
                UnderlyingPrice = underlyingPrice ?? "0",
                Basis = (_venue ?? "").Equals("Equity", StringComparison.OrdinalIgnoreCase) ||
                    new HashSet<string> { "TXA", "TXC", "TXM", "TXR" }.Contains(streamingBody?.Market ?? "") ?
                    "0" : streamingBody?.Basis ?? "0",
                OpenInterest = streamingBody?.Poi,
                ContractSize = (_venue ?? "").Equals("Derivative", StringComparison.OrdinalIgnoreCase) ?
                    _instrumentDetail?.ContractSize ?? string.Empty :
                    string.Empty,
                LastNavPerShare = _instrumentDetail?.LastNavPerShare ?? "0",
                Objective = "",
                AssetClassFocus = _instrumentDetail?.AssetClassFocus ?? "",
                ExpenseRatioPercentage = "0",
                StrikePrice = strikePrice ?? string.Empty,
                TheoreticalPrice = "0",
                IntrinsicValue = "0",
                ImpliedVolatilityPercentage = "0"
            }
        };
    }

    public WarrantData MapWarrants(StreamingBody? streamingBody)
    {
        var exercisePrice = _instrument?.ExercisePrice;
        var exerciseDate = _instrument?.ExerciseDate ?? "";
        var underlyingPrice = streamingBody?.Underlying;
        var daysToExercise = DataManipulation.CalculateDaysUntilDate(exerciseDate);
        exerciseDate = DataManipulation.ToDayMonthYear(exerciseDate);
        var maturityDate = exerciseDate;
        var moneyness = "";

        if (!string.IsNullOrEmpty(exercisePrice) && !string.IsNullOrEmpty(underlyingPrice))
        {
            var exercisePriceDecimal = decimal.Parse(exercisePrice);
            var underlyingPriceDecimal = decimal.Parse(underlyingPrice);

            switch (exercisePriceDecimal.CompareTo(underlyingPriceDecimal))
            {
                case -1: // Exercise price is less than the underlying price
                    moneyness = MoneynessConstants.ITM;
                    break;
                case 0: // Exercise price is equal to the underlying price
                    moneyness = MoneynessConstants.ATM;
                    break;
                case 1: // Exercise price is greater than the underlying price
                    moneyness = MoneynessConstants.OTM;
                    break;
            }
        }

        return new WarrantData
        {
            ExerciseDate = exerciseDate,
            DaysToExercise = daysToExercise,
            MaturityDate = maturityDate,
            Moneyness = moneyness
        };
    }

    public WarrantData MapDWs(StreamingBody? streamingBody)
    {
        var maturityDate = DataManipulation.ToDayMonthYear(_instrument?.MaturityDate ?? "");

        var lastTradingDate = "";
        if (DataManipulation.TryParseDateTime(_instrument?.LastTradingDate, out var parsedDate))
        {
            if (parsedDate != DateTime.MinValue)
            {
                lastTradingDate = _instrument?.LastTradingDate ?? string.Empty;
            }
        }
        var daysToLastTrade = DataManipulation.CalculateDaysUntilDate(lastTradingDate);
        lastTradingDate = DataManipulation.ToDayMonthYear(lastTradingDate);
        var moneyness = "";

        var underlyingPrice = streamingBody?.Price;
        var strikePrice = _instrument?.ExercisePrice;

        var direction = _instrument?.Direction;
        if (
            !string.IsNullOrEmpty(underlyingPrice)
            && !string.IsNullOrEmpty(strikePrice)
            && !string.IsNullOrEmpty(direction)
        )
        {
            var currentPriceDecimal = decimal.Parse(underlyingPrice);
            var strikePriceDecimal = decimal.Parse(strikePrice);

            switch (currentPriceDecimal.CompareTo(strikePriceDecimal))
            {
                case -1: // Current price is less than the strike price
                    if (direction == "Put")
                        moneyness = MoneynessConstants.ITM;
                    else if (direction == "Call")
                        moneyness = MoneynessConstants.OTM;
                    break;
                case 0: // Current price is equal to the strike price
                    moneyness = MoneynessConstants.ATM;
                    break;
                case 1: // Current price is greater than the strike price
                    if (direction == "Put")
                        moneyness = MoneynessConstants.OTM;
                    else if (direction == "Call")
                        moneyness = MoneynessConstants.ITM;
                    break;
            }
        }

        return new WarrantData
        {
            MaturityDate = maturityDate,
            LastTradingDate = lastTradingDate,
            DaysToLastTrade = daysToLastTrade,
            Moneyness = moneyness
        };
    }
}

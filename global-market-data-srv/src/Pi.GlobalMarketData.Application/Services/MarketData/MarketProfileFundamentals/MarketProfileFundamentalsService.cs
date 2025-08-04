using System.Globalization;
using Pi.GlobalMarketData.Application.Constants;
using Pi.GlobalMarketData.Application.Utils;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Domain.Models.Response;

namespace Pi.GlobalMarketData.Application.Services.MarketData.MarketProfileFundamentals;

public static class MarketProfileFundamentalsService
{
    public static MarketProfileFundamentalsResponse GetResult(
        MorningStarStocks? morningStarStocks,
        MorningStarEtfs? morningStarEtfs,
        StreamingBody? streamingBody
    )
    {
        var morningStarStock = morningStarStocks ?? new MorningStarStocks();
        var streaming = streamingBody ?? new StreamingBody();

        var industry = morningStarStock.Industry;
        var sector = morningStarStock.Sector;
        var country = DataManipulation.GetCountryName(morningStarStock.Country);
        var priceToEarningsRatio =
            morningStarStock.PriceToEarningsRatio.ToString(CultureInfo.InvariantCulture)
            ?? string.Empty;

        var priceToBookRatio =
            morningStarStock.PriceToBookRatio.ToString(CultureInfo.InvariantCulture)
            ?? string.Empty;
        var priceToSalesRatio =
            morningStarStock.PriceToSalesRatio.ToString(CultureInfo.InvariantCulture)
            ?? string.Empty;
        var shareFreeFloat = "0";
        var lastNavPerShare = "0";
        var objective = string.Empty;
        var assetClassFocus = string.Empty;
        var expenseRatioPercentage = "0";

        string? dividendYieldPercentage;
        string? exDividendDate;
        string units;
        string convertedMarketCapitalization;

        if (morningStarEtfs != null)
        {
            _ = double.TryParse(morningStarEtfs.MarketCap, out var marketCap);
            convertedMarketCapitalization =
                Math.Round(marketCap / 1000000000.0, 3, MidpointRounding.AwayFromZero)
                    .ToString(CultureInfo.InvariantCulture) ?? "0.000";

            _ = double.TryParse(morningStarEtfs.Dividend, out var dividend);
            dividendYieldPercentage =
                dividend.ToString(CultureInfo.InvariantCulture) ?? string.Empty;

            exDividendDate = DateTime
                .Parse(morningStarEtfs.ExDividendDate ?? string.Empty, new CultureInfo("en-US"))
                .ToString(DataFormat.DayMonthYearFormat);

            units = morningStarEtfs.Currency ?? string.Empty;

            lastNavPerShare = morningStarEtfs.LatestNAV ?? "0";

            objective = morningStarEtfs.Category ?? string.Empty;

            assetClassFocus = morningStarEtfs.AssetClassFocus ?? string.Empty;

            _ = double.TryParse(morningStarEtfs.ExpenseRatio, out var expenseRatio);
            expenseRatioPercentage =
                (expenseRatio / 100).ToString(CultureInfo.InvariantCulture) ?? "0";
        }
        else
        {
            convertedMarketCapitalization =
                Math.Round(
                        morningStarStock.MarketCapitalization / 1000000000.0,
                        3,
                        MidpointRounding.AwayFromZero
                    )
                    .ToString(CultureInfo.InvariantCulture) ?? "0.000";

            shareFreeFloat = (morningStarStock.ShareFreeFloat * 100).ToString(
                CultureInfo.InvariantCulture
            );

            exDividendDate = DataManipulation.DateToString(
                morningStarStock.ExDividendDate,
                DataFormat.DayMonthYearFormat
            );

            units = morningStarStock.Units;

            dividendYieldPercentage =
                Math.Round(morningStarStock.DividendYield * 100, 2)
                    .ToString(CultureInfo.InvariantCulture) ?? string.Empty;
        }

        return new MarketProfileFundamentalsResponse
        {
            Code = "0",
            Message = string.Empty,
            Response = new ProfileFundamentalsResponse
            {
                MarketCapitalization = convertedMarketCapitalization ?? "0.000",
                ShareFreeFloat = shareFreeFloat,
                Industry = industry,
                Sector = sector,
                Country = country,
                PriceToEarningsRatio = priceToEarningsRatio,
                PriceToBookRatio = priceToBookRatio,
                PriceToSalesRatio = priceToSalesRatio,
                DividendYieldPercentage = dividendYieldPercentage,
                ExDividendDate = exDividendDate,
                Units = units,
                Source = StockDetail.MorningStar.Value,
                UnderlyingSymbol = string.Empty,
                IsClickable = false,
                UnderlyingVenue = string.Empty,
                UnderlyingInstrumentType = string.Empty,
                UnderlyingInstrumentCategory = string.Empty,
                UnderlyingLogo = string.Empty,
                ExerciseRatio = "0 : 0",
                ExercisePrice = "0",
                ExerciseDate = string.Empty,
                DaysToExercise = string.Empty,
                Moneyness = streaming.Moneyness ?? string.Empty,
                Direction = string.Empty,
                Multiplier = "0",
                LastTradingDate = string.Empty,
                DaysToLastTrade = string.Empty,
                MaturityDate = string.Empty,
                IssuerSeries = string.Empty,
                ForeignCurrency = string.Empty,
                ConversionRatio = "0 : 0",
                UnderlyingPrice = streaming.Underlying ?? "0",
                Basis = streaming.Basis ?? "0",
                OpenInterest = "0",
                ContractSize = string.Empty,
                LastNavPerShare = lastNavPerShare,
                Objective = objective,
                AssetClassFocus = assetClassFocus,
                ExpenseRatioPercentage = expenseRatioPercentage,
                StrikePrice = "0",
                TheoreticalPrice = "0",
                IntrinsicValue = "0",
                ImpliedVolatilityPercentage = "0"
            }
        };
    }
}

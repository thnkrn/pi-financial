using System.Globalization;
using Pi.Client.SetMarketData.Model;
using Pi.SetService.Application.Models;
using Pi.SetService.Application.Options;
using Pi.SetService.Application.Utils;
using Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;
using MarketStatus = Pi.SetService.Domain.AggregatesModel.InstrumentAggregate.MarketStatus;

namespace Pi.SetService.Infrastructure.Factories;

public static class MarketFactory
{
    public static CorporateAction NewCorporateAction(string caType, string date)
    {
        return new CorporateAction
        {
            CaType = caType,
            Date = DateOnly.Parse(date, CultureInfo.InvariantCulture)
        };
    }

    public static InstrumentProfile NewInstrumentProfile(PiSetMarketDataDomainModelsResponseTickerBody ticker)
    {
        return new InstrumentProfile
        {
            FriendlyName = ticker.FriendlyName,
            InstrumentCategory = ticker.InstrumentCategory,
            Symbol = ticker.Symbol,
            Logo = ticker.Logo
        };
    }

    public static EquityInstrument NewEquityInstrument(string symbol,
        PiSetMarketDataDomainModelsResponseInstrumentInfoResponse instrumentInfo,
        PiSetMarketDataDomainModelsResponseProfileOverviewResponse? profileOverview,
        PiSetMarketDataDomainModelsResponseTickerBody ticker)
    {
        List<CorporateAction> corporateActions = [];
        if (profileOverview != null)
            corporateActions = (from action in profileOverview.CorporateActions
                                where !string.IsNullOrEmpty(action.Type) && !string.IsNullOrEmpty(action.Date)
                                select NewCorporateAction(action.Type, action.Date)).ToList();

        return new EquityInstrument
        {
            Symbol = symbol,
            IsNew = instrumentInfo.IsNew,
            Profile = new InstrumentProfile
            {
                Symbol = symbol,
                Logo = ticker.Logo,
                FriendlyName = ticker.FriendlyName,
                InstrumentCategory = ticker.InstrumentCategory
            },
            CorporateActions = corporateActions,
            TradingDetail = NewTradingDetail(ticker)
        };
    }

    public static TradingDetail NewTradingDetail(PiSetMarketDataDomainModelsResponseTickerBody ticker)
    {
        return new TradingDetail
        {
            Price = ParseDecimal(ticker.Price),
            High = ParseDecimal(ticker.High24H),
            Low = ParseDecimal(ticker.Low24H),
            Open = ParseDecimal(ticker.Open),
            PrevClose = ParseDecimal(ticker.PreClose),
            Volume = int.TryParse(ticker.Volume, out var parsed) ? parsed : 0,
            Ceiling = ParseDecimal(ticker.Ceiling),
            Floor = ParseDecimal(ticker.Floor)
        };
    }

    public static MarketStatus NewMarketStatus(SetTradingOptions options, string marketStatus)
    {
        var thNow = DateTimeHelper.ThNow();
        var todayStr = thNow.ToString(DateTimeHelper.DateFormat);
        var forceStart = DateTime.ParseExact(todayStr + " " + options.MaintenanceStartTime,
            DateTimeHelper.DateTimeFormat, CultureInfo.InvariantCulture);
        var forceEnd = DateTime.ParseExact(todayStr + " " + options.MaintenanceEndTime, DateTimeHelper.DateTimeFormat,
            CultureInfo.InvariantCulture);
        if (thNow >= forceStart && thNow < forceEnd) return MarketStatus.Maintenance;

        return marketStatus.ToLower().Replace("-", "") switch
        {
            "open1" or "preopen1" or "preopen" or "intermission" or "open2" or "preopen2" or "preclose" => MarketStatus
                .Open,
            "offhour" => MarketStatus.OffHour,
            "closed" or "close" => MarketStatus.Closed,
            _ => throw new ArgumentOutOfRangeException(marketStatus)
        };
    }

    public static CeilingFloor NewCeilingFloor(PiSetMarketDataDomainModelsResponseTickerBody ticker)
    {
        return new CeilingFloor
        {
            Symbol = ticker.Symbol,
            Floor = Convert.ToDecimal(ticker.Floor),
            Ceiling = Convert.ToDecimal(ticker.Ceiling)
        };
    }

    private static decimal ParseDecimal(string input)
    {
        return decimal.TryParse(input, out var parsedPrice) ? parsedPrice : 0;
    }
}
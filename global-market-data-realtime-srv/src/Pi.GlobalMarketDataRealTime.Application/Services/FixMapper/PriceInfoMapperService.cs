using System.Globalization;
using Pi.GlobalMarketDataRealTime.Application.Constants;
using Pi.GlobalMarketDataRealTime.Domain.Entities;
using Pi.GlobalMarketDataRealTime.Domain.Models.Fix;

namespace Pi.GlobalMarketDataRealTime.Application.Services.FixMapper;

public static class PriceInfoMapperService
{
    public static PriceInfo Map(Entry entry)
    {
        DateTime? entryDate = null;
        string? price = null;
        string? currency = null;
        string? auctionPrice = null;
        string? auctionVolume = null;
        string? open = null;
        string? high24H = null;
        string? low24H = null;
        string? high52W = null;
        string? low52W = null;
        string? priceChanged = null;
        string? priceChangedRate = null;
        string? volume = null;
        string? amount = null;
        string? preClose = null;
        string? status = null;
        string? totalAmount = null;
        string? totalAmountK = null;
        string? totalVolume = null;
        string? totalVolumeK = null;

        switch (entry.MdEntryType)
        {
            case FixMessageType.Trade:
                entryDate = entry.MdEntryTime;
                price = entry.MdEntryPx.ToString();
                high24H = entry.MdEntryPx.ToString();
                low24H = entry.MdEntryPx.ToString();
                volume = entry.MdEntrySize.ToString();
                amount = (entry.MdEntryPx * entry.MdEntrySize).ToString();
                break;
            case FixMessageType.OpeningPrice:
                open = entry.MdEntryPx.ToString();
                break;
            case FixMessageType.ClosingPrice:
                preClose = entry.MdEntryPx.ToString();
                break;
            case FixMessageType.B:
                totalVolume = entry.MdEntrySize.ToString();
                totalVolumeK = (entry.MdEntrySize / 1000).ToString();
                break;
        }

        return new PriceInfo
        {
            EntryDate = entryDate,
            Price = price,
            Currency = currency,
            AuctionPrice = auctionPrice,
            AuctionVolume = auctionVolume,
            Open = open,
            High24H = high24H,
            Low24H = low24H,
            High52W = high52W,
            Low52W = low52W,
            PriceChanged = priceChanged,
            PriceChangedRate = priceChangedRate,
            Volume = volume,
            Amount = amount,
            PreClose = preClose,
            Status = status,
            TotalAmount = totalAmount,
            TotalAmountK = totalAmountK,
            TotalVolume = totalVolume,
            TotalVolumeK = totalVolumeK
        };
    }

    public static PriceInfo MapToDatabase(PriceInfo mongoData, PriceInfo fixData)
    {
        var properties = typeof(PriceInfo).GetProperties();

        fixData.Id = mongoData.Id;
        fixData.Symbol = mongoData.Symbol;

        var timeDifference = mongoData.EntryDate - fixData.EntryDate;

        if (timeDifference?.TotalDays <= 1)
        {
            fixData.High24H =
                double.Parse(fixData.High24H ?? "0") > double.Parse(mongoData.High24H ?? "0")
                    ? fixData.High24H
                    : mongoData.High24H;

            fixData.Low24H =
                double.Parse(fixData.Low24H ?? "0") <= double.Parse(mongoData.Low24H ?? "0")
                    ? fixData.Low24H
                    : mongoData.Low24H;
        }

        foreach (var property in properties)
        {
            var newValue = property.GetValue(fixData);
            if (newValue != null) property.SetValue(mongoData, newValue);
        }

        if (mongoData.TotalVolume != null)
        {
            mongoData.TotalAmount = (
                double.Parse(mongoData.Price ?? "0") * double.Parse(mongoData.TotalVolume ?? "0")
            ).ToString(CultureInfo.InvariantCulture);
            mongoData.TotalAmountK = (
                double.Parse(mongoData.Price ?? "0")
                * double.Parse(mongoData.TotalVolume ?? "0")
                / 1000
            ).ToString(CultureInfo.InvariantCulture);
        }

        return mongoData;
    }
}
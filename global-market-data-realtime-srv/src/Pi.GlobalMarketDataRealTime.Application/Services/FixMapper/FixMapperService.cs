using Newtonsoft.Json;
using Pi.GlobalMarketDataRealTime.Application.Constants;
using Pi.GlobalMarketDataRealTime.Application.Interfaces;
using Pi.GlobalMarketDataRealTime.Domain.Entities;
using Pi.GlobalMarketDataRealTime.Domain.Models.Fix;
using Pi.GlobalMarketDataRealTime.Domain.Models.Redis;

namespace Pi.GlobalMarketDataRealTime.Application.Services.FixMapper;

// ReSharper disable once UnusedType.Global
public class FixMapperService : IFixMapperService
{
    public async Task MapToDatabase(
        FixMessage data,
        CallbackOrderBookMessageAsync callbackOrderBookMessageAsync,
        CallbackPriceInfoMessageAsync callbackPriceInfoMessageAsync,
        CallbackPublicTradeMessageAsync callbackPublicTradeMessageAsync
    )
    {
        FixData? result = null;
        var message = data.Message;

        if (message != null) result = FixData.FromJson(message);

        OrderBook? orderBook = null;
        PriceInfo? priceInfo = null;
        PublicTrade? publicTrade = null;
        var symbol = result?.Symbol ?? "";

        foreach (var entry in result?.Entries ?? [])
            switch (entry.MdEntryType)
            {
                case FixMessageType.Bid:
                case FixMessageType.Offer:
                    orderBook = OrderBookMapperService.Map(entry);
                    break;
                case FixMessageType.Trade:
                    publicTrade = PublicTradeMapperService.Map(entry);
                    priceInfo = PriceInfoMapperService.Map(entry);
                    break;
                case FixMessageType.OpeningPrice:
                case FixMessageType.ClosingPrice:
                case FixMessageType.B:
                    priceInfo = PriceInfoMapperService.Map(entry);
                    break;
            }

        await callbackOrderBookMessageAsync(orderBook, symbol);
        await callbackPriceInfoMessageAsync(priceInfo, symbol);
        await callbackPublicTradeMessageAsync(publicTrade, symbol);
    }

    public RedisValueResult MapToCache(PriceInfo message, string? currentCacheValue)
    {
        var currentPriceInfo = !string.IsNullOrEmpty(currentCacheValue)
            ? JsonConvert.DeserializeObject<PriceInfo>(currentCacheValue)
            : null;

        currentPriceInfo ??= message;

        // currentPriceInfo.AuctionPrice ราคาก่อนตลาดเปิด
        currentPriceInfo.AuctionPrice = null;
        // currentPriceInfo.AuctionVolume ราคาก่อนตลาดเปิด
        currentPriceInfo.AuctionVolume = null;

        // currentPriceInfo.High24H ราคาสูงสุดใน 24 ชั่วโมง
        if (currentPriceInfo.High24H == null)
            currentPriceInfo.High24H = message.Price;
        else
            currentPriceInfo.High24H =
                double.Parse(currentPriceInfo.High24H ?? "0")
                > double.Parse(message.Price ?? "0")
                    ? currentPriceInfo.High24H
                    : message.Price;

        // currentPriceInfo.Low24H ราคาต่ำสุดใน 24 ชั่วโมง
        if (currentPriceInfo.Low24H == null)
            currentPriceInfo.Low24H = message.Price;
        else
            currentPriceInfo.Low24H =
                double.Parse(currentPriceInfo.Low24H ?? "0")
                <= double.Parse(message.Price ?? "0")
                    ? currentPriceInfo.Low24H
                    : message.Price;

        // currentPriceInfo.High52W ราคาสูงสุดใน 1 สัปดาห์
        currentPriceInfo.High52W = null;
        // currentPriceInfo.Low52W ราคาต่ำสุดใน 1 สัปดาห์
        currentPriceInfo.Low52W = null;

        // currentPriceInfo.PriceChanged ราคาปปัจจุบัน เทียบราคาปิดของเมื่อวาน
        currentPriceInfo.PriceChanged = null;
        // currentPriceInfo.PriceChangedRate ราคาปปัจจุบัน เทียบราคาปิดของเมื่อวาน คิดเป็นเปอร์เซ็น
        currentPriceInfo.PriceChangedRate = null;

        return new RedisValueResult
        {
            RedisChannel = RedisChannel.DistributedCache,
            RedisValue = currentPriceInfo
        };
    }
}
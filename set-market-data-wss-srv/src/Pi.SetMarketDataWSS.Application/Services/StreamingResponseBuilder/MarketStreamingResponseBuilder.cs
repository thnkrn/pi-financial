using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;
using Pi.SetMarketDataWSS.Application.Helpers;
using Pi.SetMarketDataWSS.Application.Interfaces.StreamingResponseBuilder;
using Pi.SetMarketDataWSS.Application.Services.Constants;
using Pi.SetMarketDataWSS.Application.Services.Models.StreamingResponseBuilder;
using Pi.SetMarketDataWSS.Application.Utils;
using Pi.SetMarketDataWSS.Domain.Entities;
using Pi.SetMarketDataWSS.Domain.Models.Response;
using PublicTrade = Pi.SetMarketDataWSS.Application.Services.Models.ItchOrderBookMapper.PublicTrade;

namespace Pi.SetMarketDataWSS.Application.Services.StreamingResponseBuilder;

public class MarketStreamingResponseBuilder : IMarketStreamingResponseBuilder
{
    private const string PreOpenStatus = "pre-open";
    private readonly AsyncLocal<BuilderContext> _context = new();
    private readonly ILogger<MarketStreamingResponseBuilder> _logger;

    private readonly ObjectPool<MarketStreamingResponse> _marketStreamingResponsePool =
        new DefaultObjectPool<MarketStreamingResponse>(new DefaultPooledObjectPolicy<MarketStreamingResponse>());

    private readonly ObjectPool<StreamingBody> _streamingBodyPool =
        new DefaultObjectPool<StreamingBody>(new DefaultPooledObjectPolicy<StreamingBody>());

    private readonly ObjectPool<StreamingResponse> _streamingResponsePool =
        new DefaultObjectPool<StreamingResponse>(new DefaultPooledObjectPolicy<StreamingResponse>());

    /// <summary>
    /// </summary>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public MarketStreamingResponseBuilder(ILogger<MarketStreamingResponseBuilder> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IMarketStreamingResponseBuilder WithOrderBookId(string orderBookId, string messageType)
    {
        if (string.IsNullOrEmpty(orderBookId))
            throw new ArgumentException("OrderBookId cannot be null or empty", nameof(orderBookId));

        _context.Value = new BuilderContext
        {
            OrderBookId = orderBookId,
            MessageType = messageType
        };
        return this;
    }

    public Task<IMarketStreamingResponseBuilder> FetchDataAsync(FetchDataParams fetchDataParams)
    {
        var context = _context.Value ?? throw new InvalidOperationException("OrderBookId cannot be null or empty");

        try
        {
            context.OriginalPriceInfo =
                DeserializeOrDefault<PriceInfo>(fetchDataParams.OriginalPriceInfoCached ?? string.Empty);
            context.PriceInfo = DeserializeOrDefault<PriceInfo>(fetchDataParams.PriceInfoCached ?? string.Empty);
            context.MarketByPrice =
                DeserializeOrDefault<OrderBook>(fetchDataParams.MarketByPriceCached ?? string.Empty);
            context.PublicTrades = DeserializePublicTradesOrDefault(fetchDataParams.PublicTradeCached ?? string.Empty);
            context.OrderBookState =
                DeserializeOrDefault<MarketStatus>(fetchDataParams.OrderBookStateCached ?? string.Empty);
            context.InstrumentDetail =
                DeserializeOrDefault<InstrumentDetail>(fetchDataParams.InstrumentDetailCached ?? string.Empty);
            context.MarketDirectory =
                DeserializeOrDefault<MarketDirectory>(fetchDataParams.MarketDirectoryCached ?? string.Empty);
            context.OpenInterest =
                DeserializeOrDefault<OpenInterest>(fetchDataParams.OpenInterestCached ?? string.Empty);
            context.StreamingBody =
                DeserializeOrDefault<MarketStreamingResponse>(fetchDataParams.StreamingBody ?? string.Empty);
            context.UnderlyingStreamingBody =
                DeserializeOrDefault<MarketStreamingResponse>(fetchDataParams.UnderlyingStreamingBody ?? string.Empty);
            context.DataFetched = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deserializing data for OrderBookId: {OrderBookId}", context.OrderBookId);
            throw new InvalidOperationException($"Error deserializing data for OrderBookId: {context.OrderBookId}");
        }

        return Task.FromResult<IMarketStreamingResponseBuilder>(this);
    }

    public MarketStreamingResponse Build(int decimalsInPrice)
    {
        MarketStreamingResponse response;

        var context = _context.Value;
        if (context is not { DataFetched: true })
            throw new InvalidOperationException("Data must be fetched before building the response");

        var validAggressor = new HashSet<string> { "A", "B" };
        var isValidLastTrade = DateTime.TryParse(context.OriginalPriceInfo?.LastTrade,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var lastTrade);

        if (context.StreamingBody is { Response.Data.Count: > 0 })
        {
            response = context.StreamingBody;
        }
        else
        {
            var streamingBody = _streamingBodyPool.Get();
            var streamingResponse = _streamingResponsePool.Get();
            response = _marketStreamingResponsePool.Get();
            streamingResponse.Data = [streamingBody];
            response.Code = "0";
            response.Op = "Streaming";
            response.Message = "Success";
            response.Response = streamingResponse;
        }

        UpdateOriginalPriceInfo(context, response, isValidLastTrade, lastTrade);
        UpdateOrderBookState(context, response);
        UpdateMarketByPrice(context, response, decimalsInPrice);

        if (context.MarketDirectory != null)
            response.Response.Data[0].Market = context.MarketDirectory.MarketDescription;

        if (context.OpenInterest != null)
            response.Response.Data[0].Poi = context.OpenInterest.POI.ToString();

        if (context.UnderlyingStreamingBody is { Response.Data.Count: > 0 })
            response.Response.Data[0].Underlying = context.UnderlyingStreamingBody.Response.Data[0].Price ?? "0.00";

        response.Response.Data[0].InstrumentType = string.Empty;
        response.Response.Data[0].ToLastTrade = 0;
        response.Response.Data[0].Moneyness = string.Empty;
        response.Response.Data[0].MaturityDate = string.Empty;
        response.Response.Data[0].Multiplier = "0";
        response.Response.Data[0].IntrinsicValue = "0";
        response.Response.Data[0].PSettle = "0";

        if (!string.IsNullOrEmpty(context.MessageType))
            switch (context.MessageType[0])
            {
                case ItchMessageType.i:
                    UpdateTypeIResponse(context, response, decimalsInPrice, validAggressor);
                    UpdateYieldAndSettle(context, response);
                    break;
                case ItchMessageType.I:
                    UpdateTypeIUpperResponse(context, response, decimalsInPrice);
                    UpdateYieldAndSettle(context, response);
                    break;
                case ItchMessageType.Q:
                    UpdateTypeQUpperResponse(context, response, decimalsInPrice);
                    UpdateYieldAndSettle(context, response);
                    break;
                case ItchMessageType.J:
                    UpdateTypeJUpperResponse(context, response, decimalsInPrice);
                    UpdateYieldAndSettle(context, response);
                    break;
                case ItchMessageType.k:
                    UpdateTypeKResponse(context, response, decimalsInPrice);
                    UpdateYieldAndSettle(context, response);
                    break;
                case ItchMessageType.Z:
                    UpdateTypeZUpperResponse(context, response, decimalsInPrice);
                    UpdateYieldAndSettle(context, response);
                    break;
            }

        return response;
    }

    private static void UpdateMarketByPrice(BuilderContext context, MarketStreamingResponse response,
        int decimalsForPrice)
    {
        if (context.MarketByPrice != null && response is { Response.Data.Count: > 0 })
            response.Response.Data[0].OrderBook = new StreamingOrderBook
            {
                Bid = context.MarketByPrice.Bid?
                    .Select(bidAsk => new List<string>
                    {
                        bidAsk.Price.FormatDecimals(decimalsForPrice, true, 2),
                        bidAsk.Quantity ?? string.Empty
                    }).ToList(),
                Offer = context.MarketByPrice.Offer?
                    .Select(bidAsk => new List<string>
                    {
                        bidAsk.Price.FormatDecimals(decimalsForPrice, true, 2),
                        bidAsk.Quantity ?? string.Empty
                    }).ToList()
            };
    }

    private static void UpdateOrderBookState(BuilderContext context, MarketStreamingResponse response)
    {
        if (context.OrderBookState != null && response is { Response.Data.Count: > 0 })
        {
            response.Response.Data[0].IsProjected =
                context.OrderBookState.Status?.StartsWith(PreOpenStatus,
                    StringComparison.OrdinalIgnoreCase) ??
                false;
            response.Response.Data[0].Status = context.OrderBookState.Status;
        }
    }

    private static void UpdateOriginalPriceInfo(BuilderContext context, MarketStreamingResponse response,
        bool isValidLastTrade,
        DateTime lastTrade)
    {
        if (context.OriginalPriceInfo != null && response is { Response.Data.Count: > 0 })
        {
            response.Response.Data[0].Symbol = context.OriginalPriceInfo.Symbol;
            response.Response.Data[0].Venue =
                VenueHelper.GetVenue(context.OriginalPriceInfo.SecurityType ?? string.Empty);
            response.Response.Data[0].SecurityType = context.OriginalPriceInfo.SecurityType;
            response.Response.Data[0].LastTrade = isValidLastTrade
                ? lastTrade.ToString("dd/MM/yyyy")
                : context.OriginalPriceInfo.LastTrade;
            response.Response.Data[0].ExercisePrice = context.OriginalPriceInfo.ExercisePrice ?? "0";
        }
    }

    private static void UpdateTypeZUpperResponse(BuilderContext context, MarketStreamingResponse response,
        int decimalsForPrice)
    {
        if (context.PriceInfo != null && response is { Response.Data.Count: > 0 })
        {
            var price = context.PriceInfo.Price ?? "0";
            if (long.TryParse(price, out var parseValue) && parseValue > 0)
                response.Response.Data[0].Price = parseValue.ToString().FormatDecimals(decimalsForPrice, true, 2);

            response.Response.Data[0].AuctionPrice =
                context.PriceInfo.AuctionPrice.FormatDecimals(decimalsForPrice, true, 2);

            // Basis related to price
            response.Response.Data[0].Basis = CalculateBasis(
                price.FormatDecimals(decimalsForPrice, true, 2),
                context.UnderlyingStreamingBody?.Response.Data?[0].Price ?? "0");
        }
    }

    private static void UpdateTypeKResponse(BuilderContext context, MarketStreamingResponse response,
        int decimalsForPrice)
    {
        if (context.PriceInfo != null && response is { Response.Data.Count: > 0 })
        {
            response.Response.Data[0].Ceiling =
                context.PriceInfo.Ceiling.FormatDecimals(decimalsForPrice, true, 2);
            response.Response.Data[0].Floor =
                context.PriceInfo.Floor.FormatDecimals(decimalsForPrice, true, 2);
        }
    }

    private static void UpdateTypeQUpperResponse(BuilderContext context, MarketStreamingResponse response,
        int decimalsForPrice)
    {
        if (context.PriceInfo != null && response is { Response.Data.Count: > 0 })
            response.Response.Data[0].PreClose =
                context.PriceInfo.PreClose.FormatDecimals(decimalsForPrice, true, 2);
    }

    private static void UpdateTypeJUpperResponse(BuilderContext context, MarketStreamingResponse response,
        int decimalsForPrice)
    {
        if (context.PriceInfo != null && response is { Response.Data.Count: > 0 })
        {
            var price = context.PriceInfo.Price ?? "0";
            if (long.TryParse(price, out var parseValue) && parseValue > 0)
                response.Response.Data[0].Price = parseValue.ToString().FormatDecimals(decimalsForPrice, true, 2);

            response.Response.Data[0].Open =
                context.PriceInfo.Open.FormatDecimals(decimalsForPrice, true, 2);
            response.Response.Data[0].High24H =
                context.PriceInfo.High24H.FormatDecimals(decimalsForPrice, true, 2);
            response.Response.Data[0].Low24H =
                context.PriceInfo.Low24H.FormatDecimals(decimalsForPrice, true, 2);
            response.Response.Data[0].PriceChanged =
                context.PriceInfo.PriceChanged.FormatDecimals(decimalsForPrice, true, 2);
            response.Response.Data[0].PriceChangedRate = context.PriceInfo.CalculatedPriceChangedRate
                ? context.PriceInfo.PriceChangedRate
                : context.PriceInfo.PriceChangedRate.FormatDecimals(decimalsForPrice, true, 2);
            response.Response.Data[0].PreClose =
                context.PriceInfo.PreClose.FormatDecimals(decimalsForPrice, true, 2);
            response.Response.Data[0].TotalAmount =
                context.PriceInfo.TotalAmount.FormatDecimals(decimalsForPrice, true, 2);
            response.Response.Data[0].TotalVolume = context.PriceInfo.TotalVolume;
            response.Response.Data[0].TotalAmountK =
                context.PriceInfo.TotalAmountK.FormatDecimals(decimalsForPrice, true);
            response.Response.Data[0].TotalVolumeK = context.PriceInfo.TotalVolumeK;

            // Basis related to Price
            response.Response.Data[0].Basis = CalculateBasis(
                price.FormatDecimals(decimalsForPrice, true, 2),
                context.UnderlyingStreamingBody?.Response.Data?[0].Price ?? "0");
        }
    }

    private static void UpdateTypeIUpperResponse(BuilderContext context, MarketStreamingResponse response,
        int decimalsForPrice)
    {
        if (context.PriceInfo != null && response is { Response.Data.Count: > 0 })
        {
            response.Response.Data[0].AuctionPrice =
                context.PriceInfo.AuctionPrice.FormatDecimals(decimalsForPrice, true, 2);
            response.Response.Data[0].AuctionVolume = context.PriceInfo.AuctionVolume;
            response.Response.Data[0].Open =
                context.PriceInfo.Open.FormatDecimals(decimalsForPrice, true, 2);
            response.Response.Data[0].High24H =
                context.PriceInfo.High24H.FormatDecimals(decimalsForPrice, true, 2);
            response.Response.Data[0].Low24H =
                context.PriceInfo.Low24H.FormatDecimals(decimalsForPrice, true, 2);
            response.Response.Data[0].Average =
                context.PriceInfo.Average.FormatDecimals(decimalsForPrice, true);
            response.Response.Data[0].TotalAmount =
                context.PriceInfo.TotalAmount.FormatDecimals(decimalsForPrice, true, 2);
            response.Response.Data[0].TotalAmountK =
                context.PriceInfo.TotalAmountK.FormatDecimals(decimalsForPrice, true);
            response.Response.Data[0].TotalVolume = context.PriceInfo.TotalVolume;
            response.Response.Data[0].TotalVolumeK = context.PriceInfo.TotalVolumeK;
            response.Response.Data[0].Open0 =
                context.PriceInfo.Open0.FormatDecimals(decimalsForPrice, true, 2);
            response.Response.Data[0].Open1 =
                context.PriceInfo.Open1.FormatDecimals(decimalsForPrice, true, 2);
            response.Response.Data[0].Open2 =
                context.PriceInfo.Open2.FormatDecimals(decimalsForPrice, true, 2);
        }
    }

    private static void UpdateTypeIResponse(BuilderContext context, MarketStreamingResponse response,
        int decimalsForPrice,
        HashSet<string> validAggressor)
    {
        if (context.PriceInfo != null && response is { Response.Data.Count: > 0 })
        {
            response.Response.Data[0].Price =
                context.PriceInfo.Price.FormatDecimals(decimalsForPrice, true, 2);
            response.Response.Data[0].PriceChanged =
                context.PriceInfo.PriceChanged.FormatDecimals(decimalsForPrice, true, 2);
            response.Response.Data[0].PriceChangedRate = context.PriceInfo.CalculatedPriceChangedRate
                ? context.PriceInfo.PriceChangedRate
                : context.PriceInfo.PriceChangedRate.FormatDecimals(decimalsForPrice, true, 2);
            response.Response.Data[0].Volume = context.PriceInfo.Volume;
            response.Response.Data[0].Aggressor = context.PriceInfo.Aggressor;
            response.Response.Data[0].LastPriceTime = context.PriceInfo.LastPriceTime;
            response.Response.Data[0].Amount =
                context.PriceInfo.Amount.FormatDecimals(decimalsForPrice, true);
            response.Response.Data[0].AverageSell =
                context.PriceInfo.AverageSell.FormatDecimals(decimalsForPrice, true);
            response.Response.Data[0].AverageBuy =
                context.PriceInfo.AverageBuy.FormatDecimals(decimalsForPrice, true);

            if (context.UnderlyingStreamingBody is { Response.Data.Count: > 0 })
                // Basis related to Price
                response.Response.Data[0].Basis = CalculateBasis(
                    context.PriceInfo.Price.FormatDecimals(decimalsForPrice, true, 2),
                    context.UnderlyingStreamingBody.Response.Data?[0].Price ?? "0");

            if (context.PublicTrades != null)
                response.Response.Data[0].PublicTrades = context.PublicTrades
                    .Where(x => validAggressor.Contains(x.Aggressor?.Value ?? string.Empty))
                    .Select(publicTrade => new List<object>
                    {
                        publicTrade.Nanos,
                        publicTrade.GetDealTime(),
                        publicTrade.Aggressor?.Value ?? string.Empty,
                        publicTrade.Quantity.Value.ToString(),
                        publicTrade.Price.Value.ToString().FormatDecimals(decimalsForPrice, true, 2),
                        publicTrade.CalculatePriceChanged(context.PriceInfo.PreClose)
                            .FormatDecimals(decimalsForPrice, true, 2)
                    }).ToList();
        }
    }

    private static void UpdateYieldAndSettle(BuilderContext context, MarketStreamingResponse response)
    {
        if (context.PriceInfo != null && response is { Response.Data.Count: > 0 })
        {
            if (string.IsNullOrEmpty(context.PriceInfo.Yield))
                response.Response.Data[0].Yield = context.PriceInfo.Yield;

            if (string.IsNullOrEmpty(context.PriceInfo.Settle))
                response.Response.Data[0].Settle = context.PriceInfo.Settle;
        }
    }

    private T? DeserializeOrDefault<T>(string json) where T : new()
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(json) ?? default;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to deserialize {Type}. Using default value.", typeof(T).Name);
            return default;
        }
    }

    private PublicTrade[] DeserializePublicTradesOrDefault(string json)
    {
        try
        {
            return JsonConvert.DeserializeObject<PublicTrade[]>(json) ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to deserialize PublicTrades. Using default value.");
            return [];
        }
    }

    private static string CalculateBasis(string price, string underlyingPrice)
    {
        if (double.TryParse(price, out var number1) && double.TryParse(underlyingPrice, out var number2))
        {
            var result = number1 - number2;
            return result.ToString("F2");
        }

        return price;
    }
}
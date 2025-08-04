using Pi.SetMarketData.Application.Interfaces.MarketProfileOverview;
using Pi.SetMarketData.Application.Services.MarketData.MarketProfileOverview;
using Pi.SetMarketData.Application.Utils;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Domain.Models.Response;

namespace Pi.SetMarketData.Application.Services.MarketDataController.MarketProfileOverviewService;

public class MarketProfileOverviewParams
{
    public double High52W { get; set; } = 0;
    public double Low52W { get; set; } = 0;
}

public class MarketProfileOverviewService
{
    private Instrument? _instruments;
    private MarketProfileOverviewParams? _marketProfileParams;
    private SetVenueMapping? _setVenueMapping;
    private CorporateAction? _corporateAction;
    private TradingSign? _tradingSign;
    private MarketStreamingResponse? _marketStreamingResponse;
    private bool _isTfex;

    private readonly CorporateActionsMapper _corporateActionsMapper;
    private readonly TradingSignsMapper _tradingSignsMapper;

    public MarketProfileOverviewService()
    {
        _corporateActionsMapper = new CorporateActionsMapper();
        _tradingSignsMapper = new TradingSignsMapper();
    }

    public MarketProfileOverviewService WithInstruments(Instrument instruments)
    {
        _instruments = instruments;
        return this;
    }

    public MarketProfileOverviewService WithMarketProfileParams(
        MarketProfileOverviewParams marketProfileOverviewParams
    )
    {
        _marketProfileParams = marketProfileOverviewParams;
        return this;
    }

    public MarketProfileOverviewService WithVenueMapping(SetVenueMapping mapping)
    {
        _setVenueMapping = mapping;
        return this;
    }

    public MarketProfileOverviewService WithCorporateAction(CorporateAction action)
    {
        _corporateAction = action;
        return this;
    }

    public MarketProfileOverviewService WithTradingSign(TradingSign sign)
    {
        _tradingSign = sign;
        return this;
    }

    public MarketProfileOverviewService WithMarketStreamingResponse(
        MarketStreamingResponse response
    )
    {
        _marketStreamingResponse = response;
        return this;
    }

    public MarketProfileOverviewService WithIsTfex(bool isTfex)
    {
        _isTfex = isTfex;
        return this;
    }

    public MarketProfileOverviewResponse GetResult()
    {
        var marketStreaming = _marketStreamingResponse?.Response?.Data?.FirstOrDefault();
        return new MarketProfileOverviewResponse
        {
            Code = "0",
            Message = string.Empty,
            Response = new ProfileOverviewResponse
            {
                Market = DataManipulation.RemoveSpace(
                    _instruments?.InstrumentCategory ?? string.Empty
                ),
                Exchange = _setVenueMapping?.Exchange ?? string.Empty,
                ExchangeTime = string.Empty,
                LastPrice = marketStreaming?.Price ?? "0",
                PriorClose = marketStreaming?.PreClose ?? "0",
                PriceChange = marketStreaming?.PriceChanged ?? "0",
                PriceChangePercentage = marketStreaming?.PriceChangedRate ?? "0",
                MinimumOrderSize = _instruments?.MinimumOrderSize ?? string.Empty,
                High52W = _marketProfileParams?.High52W.ToString("F2"),
                Low52W = _marketProfileParams?.Low52W.ToString("F2"),
                ContractMonth = _isTfex
                    ? DataManipulation.ToMonthYear(_instruments?.LastTradingDate ?? string.Empty)
                    : string.Empty,
                Currency = _instruments?.Currency ?? string.Empty,
                CorporateActions = _corporateActionsMapper.MapCorporateAction(_corporateAction!),
                TradingSign = _tradingSignsMapper.MapTradingSigns(_tradingSign!)
            }
        };
    }
}

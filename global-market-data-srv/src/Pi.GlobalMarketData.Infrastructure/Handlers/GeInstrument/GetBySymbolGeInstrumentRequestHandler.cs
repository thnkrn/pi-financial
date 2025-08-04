using Pi.GlobalMarketData.Application.Interfaces.GeInstrument;
using Pi.GlobalMarketData.Application.Queries;
using Pi.GlobalMarketData.Domain.Models.Response;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.GlobalMarketData.Infrastructure.Handlers.GeInstrument;

public class GetBySymbolGeInstrumentRequestHandler : GetBySymbolGeInstrumentRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.GeInstrument> _geInstrumentService;

    public GetBySymbolGeInstrumentRequestHandler(
        IMongoService<Domain.Entities.GeInstrument> geInstrumentService
    )
    {
        _geInstrumentService = geInstrumentService;
    }

    protected override async Task<GetBySymbolGeInstrumentResponse> Handle(
        GetBySymbolGeInstrumentRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            string instrumentType = request.conditions.InstrumentType ?? "";
            string keyword = request.conditions.Keyword ?? "";

            var result = await _geInstrumentService.GetAllAsync();

            if (!string.IsNullOrEmpty(instrumentType))
                result = result.Where(e =>
                    (!string.IsNullOrEmpty(e.InstrumentType) ? e.InstrumentType : "").Trim().ToLower()
                    .Contains(instrumentType.Trim().ToLower()));

            if (!string.IsNullOrEmpty(keyword))
                result = result.Where(e =>
                       (!string.IsNullOrEmpty(e.Exchange) ? e.Exchange : "").Trim().ToLower().Contains(keyword.Trim().ToLower())
                    || (!string.IsNullOrEmpty(e.Name) ? e.Name : "").Trim().ToLower().Contains(keyword.Trim().ToLower())
                    || (!string.IsNullOrEmpty(e.Symbol) ? e.Symbol : "").Trim().ToLower().Contains(keyword.Trim().ToLower())
                    || (!string.IsNullOrEmpty(e.Venue) ? e.Venue : "").Trim().ToLower().Contains(keyword.Trim().ToLower())
               );

            var searchResult = result.Select(e => new InstrumentSearchList
            {
                Venue = e.Exchange,
                Symbol = e.Symbol,
                FriendlyName = e.Name,
                Logo = null,
                Price = null,
                PriceChange = null,
                PriceChangeRatio = null,
                IsFavorite = false,
                Unit = e.Currency
            }).ToList();

            var data = new MarketInstrumentSearchResponse
            {
                Code = "0",
                Message = "",
                Response = new InstrumentSearchResponse
                {
                    InstrumentCategoryList =
                    [
                        new InstrumentSearchCategoryList
                                {
                                    Order = searchResult.Count,
                                    InstrumentType = "GlobalEquity",
                                    InstrumentCategory = "GlobalStocks",
                                    InstrumentList = searchResult
                                }
                    ]
                }
            };

            return new GetBySymbolGeInstrumentResponse(data);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}

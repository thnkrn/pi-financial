using Pi.SetMarketData.Application.Interfaces.MarketInstrumentSearch;
using Pi.SetMarketData.Application.Queries.MarketInstrumentSearch;
using Pi.SetMarketData.Application.Services.MarketData.MarketInstrumentSearch;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.Domain.Models.Response;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;
using Pi.SetMarketData.Infrastructure.Interfaces.Redis;

namespace Pi.SetMarketData.Infrastructure.Handlers.MarketInstrumentSearch
{
    public class PostMarketInstrumentSearchHandler : PostMarketInstrumentSearchAbstractHandler
    {
        private readonly IMongoService<Domain.Entities.Instrument> _instrumentService;
        private readonly IMongoService<Domain.Entities.InstrumentDetail> _instrumentDetailService;
        private readonly ICacheService _cacheService;
        public PostMarketInstrumentSearchHandler(
            IMongoService<Domain.Entities.Instrument> instrumentService,
            IMongoService<Domain.Entities.InstrumentDetail> instrumentDetailService,
            ICacheService cacheService
        )
        {
            _instrumentService = instrumentService;
            _instrumentDetailService = instrumentDetailService;
            _cacheService = cacheService;
        }

        protected override async Task<PostMarketInstrumentSearchResponse> Handle(
            PostMarketInstrumentSearchRequest request,
            CancellationToken cancellationToken
        )
        {
            try
            {
                List<Domain.Entities.Instrument> selectedInstrumentsList = [];
                List<MarketStreamingResponse> marketStreamingList = [];
                List<Domain.Entities.InstrumentDetail> instrumentDetailsList = [];

                var instruments = await _instrumentService.GetAllByFilterAsync(target => target.Deprecated != true);
                var instrumentsList = request.data.InstrumentType == "" ? instruments.ToList() :
                    instruments.Where(x => x.InstrumentType == request.data.InstrumentType).ToList();
                foreach (var instrument in instrumentsList)
                {
                    if (instrument.Symbol == request.data.Keyword
                        || instrument.Venue == request.data.Keyword
                        || instrument.Symbol == request.data.Keyword
                        || instrument.Exchange == request.data.Keyword)
                    {
                        var instrumentDetail = await _instrumentDetailService.GetByFilterAsync(x => x.OrderBookId == instrument.OrderBookId)
                            ?? new Domain.Entities.InstrumentDetail();
                        var marketStreamingValue = await _cacheService.GetAsync<MarketStreamingResponse>(
                            $"{CacheKey.StreamingBody}{instrument.OrderBookId}"
                        ) ?? new MarketStreamingResponse();

                        selectedInstrumentsList.Add(instrument);
                        marketStreamingList.Add(marketStreamingValue);
                        instrumentDetailsList.Add(instrumentDetail);
                    }
                }

                var result = MarketInstrumentSearchService.GetResult(selectedInstrumentsList, marketStreamingList, instrumentDetailsList);

                return new PostMarketInstrumentSearchResponse(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}

using Pi.SetMarketData.Application.Interfaces.InstrumentDetail;
using Pi.SetMarketData.Application.Queries.InstrumentDetail;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.InstrumentDetail;

public class GetInstrumentDetailRequestHandler: GetInstrumentDetailRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.InstrumentDetail> _instrumentDInstrumentDetailService;
    
    public GetInstrumentDetailRequestHandler(IMongoService<Domain.Entities.InstrumentDetail> instrumentDInstrumentDetailService)
    {
        _instrumentDInstrumentDetailService = instrumentDInstrumentDetailService;
    }
    
    protected override async Task<GetInstrumentDetailResponse> Handle(GetInstrumentDetailRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _instrumentDInstrumentDetailService.GetAllAsync();
            return new GetInstrumentDetailResponse(result.ToList());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
using Pi.SetMarketData.Application.Queries.InstrumentDetail;
using Pi.SetMarketData.Application.Interfaces.InstrumentDetail;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.InstrumentDetail;

public class GetByIdInstrumentDetailRequestHandler : GetByIdInstrumentDetailRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.InstrumentDetail> _instrumentDetailService;

    public GetByIdInstrumentDetailRequestHandler(IMongoService<Domain.Entities.InstrumentDetail> instrumentDetailService)
    {
        _instrumentDetailService = instrumentDetailService;
    }

    protected override async Task<GetByIdInstrumentDetailResponse> Handle(GetByIdInstrumentDetailRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _instrumentDetailService.GetByIdAsync(request.id);
            return new GetByIdInstrumentDetailResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
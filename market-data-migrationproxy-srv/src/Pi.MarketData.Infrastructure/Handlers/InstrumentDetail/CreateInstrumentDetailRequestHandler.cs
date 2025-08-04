using Pi.MarketData.Application.Commands.InstrumentDetail;
using Pi.MarketData.Application.Interfaces.InstrumentDetail;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.InstrumentDetail;

public class CreateInstrumentDetailRequestHandler : CreateInstrumentDetailRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.InstrumentDetail> _instrumentDetailService;

    public CreateInstrumentDetailRequestHandler(IMongoService<Domain.Entities.InstrumentDetail> instrumentDetailService)
    {
        _instrumentDetailService = instrumentDetailService;
    }

    protected override async Task<CreateInstrumentDetailResponse> Handle(CreateInstrumentDetailRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _instrumentDetailService.CreateAsync(request.InstrumentDetail);
            return new CreateInstrumentDetailResponse(true, request.InstrumentDetail);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
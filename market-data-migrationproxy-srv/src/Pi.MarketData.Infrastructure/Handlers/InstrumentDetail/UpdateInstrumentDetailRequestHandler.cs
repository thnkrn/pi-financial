using Pi.MarketData.Application.Commands.InstrumentDetail;
using Pi.MarketData.Application.Interfaces.InstrumentDetail;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.InstrumentDetail;

public class UpdateInstrumentDetailRequestHandler : UpdateInstrumentDetailRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.InstrumentDetail> _instrumentDInstrumentDetailService;

    public UpdateInstrumentDetailRequestHandler(
        IMongoService<Domain.Entities.InstrumentDetail> instrumentDInstrumentDetailService)
    {
        _instrumentDInstrumentDetailService = instrumentDInstrumentDetailService;
    }

    protected override async Task<UpdateInstrumentDetailResponse> Handle(UpdateInstrumentDetailRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _instrumentDInstrumentDetailService.UpdateAsync(request.id, request.InstrumentDetail);
            return new UpdateInstrumentDetailResponse(true, request.InstrumentDetail);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
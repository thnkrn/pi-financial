using Pi.SetMarketData.Application.Commands.InstrumentDetail;
using Pi.SetMarketData.Application.Interfaces.InstrumentDetail;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.InstrumentDetail;

public class DeleteInstrumentDetailRequestHandler : DeleteInstrumentDetailRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.InstrumentDetail> _instrumentDetailService;

    public DeleteInstrumentDetailRequestHandler(IMongoService<Domain.Entities.InstrumentDetail> instrumentDetailService)
    {
        _instrumentDetailService = instrumentDetailService;
    }

    protected override async Task<DeleteInstrumentDetailResponse> Handle(DeleteInstrumentDetailRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _instrumentDetailService.DeleteAsync(request.id);
            return new DeleteInstrumentDetailResponse(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
using Pi.SetMarketData.Application.Commands.Instrument;
using Pi.SetMarketData.Application.Interfaces.Instrument;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.Instrument;

public class DeleteInstrumentRequestHandler : DeleteInstrumentRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.Instrument> _instrumentService;

    public DeleteInstrumentRequestHandler(IMongoService<Domain.Entities.Instrument> instrumentService)
    {
        _instrumentService = instrumentService;
    }

    protected override async Task<DeleteInstrumentResponse> Handle(DeleteInstrumentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _instrumentService.DeleteAsync(request.id);
            return new DeleteInstrumentResponse(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
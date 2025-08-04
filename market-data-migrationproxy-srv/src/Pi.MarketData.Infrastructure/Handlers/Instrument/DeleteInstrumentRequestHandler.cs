using Pi.MarketData.Application.Commands.Instrument;
using Pi.MarketData.Application.Interfaces.Instrument;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.Instrument;

public class DeleteInstrumentRequestHandler : DeleteInstrumentRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.Instrument> _instrumentService;

    public DeleteInstrumentRequestHandler(IMongoService<Domain.Entities.Instrument> instrumentService)
    {
        _instrumentService = instrumentService;
    }

    protected override async Task<DeleteInstrumentResponse> Handle(DeleteInstrumentRequest request,
        CancellationToken cancellationToken)
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
using Pi.MarketData.Application.Commands.Instrument;
using Pi.MarketData.Application.Interfaces.Instrument;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.Instrument;

public class UpdateInstrumentRequestHandler : UpdateInstrumentRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.Instrument> _instrumentService;

    public UpdateInstrumentRequestHandler(IMongoService<Domain.Entities.Instrument> instrumentService)
    {
        _instrumentService = instrumentService;
    }

    protected override async Task<UpdateInstrumentResponse> Handle(UpdateInstrumentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _instrumentService.UpdateAsync(request.id, request.Instrument);
            return new UpdateInstrumentResponse(true, request.Instrument);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
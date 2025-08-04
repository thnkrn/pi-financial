using Pi.SetMarketData.Application.Commands.Instrument;
using Pi.SetMarketData.Application.Interfaces.Instrument;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.Instrument;

public class UpdateInstrumentRequestHandler : UpdateInstrumentRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.Instrument> _instrumentService;

    public UpdateInstrumentRequestHandler(IMongoService<Domain.Entities.Instrument> instrumentService)
    {
        _instrumentService = instrumentService;
    }

    protected override async Task<UpdateInstrumentResponse> Handle(UpdateInstrumentRequest request, CancellationToken cancellationToken)
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
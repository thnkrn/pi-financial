using Pi.GlobalMarketData.Application.Commands;
using Pi.GlobalMarketData.Application.Interfaces;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.GlobalMarketData.Infrastructure.Handlers;

public class UpdateCuratedMemberRequestHandler : UpdateCuratedMemberRequestAbstractHandler
{
    private readonly IMongoService<CuratedMember> _CuratedMemberService;

    public UpdateCuratedMemberRequestHandler(IMongoService<CuratedMember> CuratedMemberService)
    {
        _CuratedMemberService = CuratedMemberService;
    }

    protected override async Task<UpdateCuratedMemberResponse> Handle(UpdateCuratedMemberRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _CuratedMemberService.UpdateAsync(request.id, request.CuratedMember);
            return new UpdateCuratedMemberResponse(true, request.CuratedMember);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
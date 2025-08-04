using MongoDB.Bson;
using Pi.GlobalMarketData.Application.Commands;
using Pi.GlobalMarketData.Application.Interfaces;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.GlobalMarketData.Infrastructure.Handlers;

public class DeleteCuratedMemberRequestHandler : DeleteCuratedMemberRequestAbstractHandler
{
    private readonly IMongoService<CuratedMember> _curatedMemberService;

    public DeleteCuratedMemberRequestHandler(IMongoService<CuratedMember> curatedMemberService)
    {
        _curatedMemberService = curatedMemberService;
    }

    protected override async Task<DeleteCuratedMemberResponse> Handle(DeleteCuratedMemberRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (ObjectId.TryParse(request.id, out var objectId))
            {
                var curatedList = await _curatedMemberService.GetByIdAsync(request.id);
                if (curatedList == null) return new DeleteCuratedMemberResponse(false, "NotFound");
                await _curatedMemberService.DeleteAsync(request.id);
                return new DeleteCuratedMemberResponse(true, null);
            }
            return new DeleteCuratedMemberResponse(false, "BadRequest");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
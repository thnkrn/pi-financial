using Pi.GlobalMarketData.Application.Commands;
using Pi.GlobalMarketData.Application.Interfaces;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.GlobalMarketData.Infrastructure.Handlers;

public class CreateCuratedMemberRequestHandler : CreateCuratedMemberRequestAbstractHandler
{
    private readonly IMongoService<CuratedMember> _curatedMemberService;

    public CreateCuratedMemberRequestHandler(IMongoService<CuratedMember> curatedMemberService)
    {
        _curatedMemberService = curatedMemberService;
    }

    protected override async Task<CreateCuratedMemberResponse> Handle(CreateCuratedMemberRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _curatedMemberService.UpsertManyAsync(request.CuratedMember, x => x.Id);
            return new CreateCuratedMemberResponse(true, request.CuratedMember);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
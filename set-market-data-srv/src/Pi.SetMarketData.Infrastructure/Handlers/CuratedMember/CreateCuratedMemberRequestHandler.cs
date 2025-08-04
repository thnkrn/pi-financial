using Pi.SetMarketData.Application.Commands;
using Pi.SetMarketData.Application.Interfaces;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers;

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
            var result = await _curatedMemberService.UpsertManyAsync(request.CuratedMember, x => x.Id);
            return new CreateCuratedMemberResponse(true, request.CuratedMember);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
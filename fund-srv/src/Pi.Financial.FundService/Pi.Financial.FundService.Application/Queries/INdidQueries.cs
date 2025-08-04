namespace Pi.Financial.FundService.Application.Queries;

public record Ndid(string ReferenceId, string RequestTime);

public interface INdidQueries
{
    Task<Ndid> Get(string custcode);
}

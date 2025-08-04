using System.Linq.Expressions;

namespace Pi.BackofficeService.Domain.SeedWork;

public interface IQueryFilter<T>
{
    List<Expression<Func<T, bool>>> GetExpressions();
}
